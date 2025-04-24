using System.Collections;
using UnityEngine;
using EditorTools;
using DebugMenu;
using FMOD.Studio;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using GameEvents;
using CustomFMODFunctions;
using FMODUnity;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public abstract class BreakableObject : Task, IBombDamageable
{
    public BreakableObjectDataSO BreakableObjectData => TaskData as BreakableObjectDataSO; //Cast to use TaskData data as BreakableObjectDataSO
    //Important data and Game events and SOs
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent), FoldoutGroup("Data")] protected GUIBreakableObjectWarning _warning;
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent), FoldoutGroup("Data")] protected Health _health;
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent), FoldoutGroup("Data")] protected Collider Collider;
    [SerializeField, FoldoutGroup("Data")] private BoolEventAsset OnSetPlayerFixing; //Set player to fixing on Interactor

    [SerializeField, FoldoutGroup("State")] private bool _startBroken; //Set player to fixing on Interactor

    //VFXs
    [SerializeField, FoldoutGroup("VFX")] private GameObject _fixingVFXPrefab;
    [SerializeField, FoldoutGroup("VFX")] protected VisualEffect _fixingVFX;
    [SerializeField, FoldoutGroup("VFX")] private GameObject _repairedVFXPrefab;
    [SerializeField, FoldoutGroup("VFX")] private VisualEffect _repairedVFX;
    [field: SerializeField, FoldoutGroup("VFX")] public Transform VFXSpawnPoint { get; private set; }

    //Fills of the breakableObject
    [field: SerializeField, FoldoutGroup("Fills")] public GameObject InitialFill { get; private set; } = null;
    [field: SerializeField, FoldoutGroup("Fills")] public GameObject MaterialPlacedFill { get; private set; } = null;
    [field: SerializeField, FoldoutGroup("Fills")] public GameObject FixedFill { get; private set; } = null;
    [field: SerializeField, FoldoutGroup("Fills")] public GameObject BrokenFill { get; private set; } = null;

    
    [Header("Fade Out Settings")]
    [SerializeField] protected float startAlpha = 1f;
    [SerializeField] protected float endAlpha = 0f;
    [SerializeField] protected float duration = 1f;
    
    public RepairState RepairStatus { get; protected set; } = RepairState.Fixed;

    //VFX Instances
    protected GameObject _fixingVFXInstance = null;
    private GameObject _repairedVFXPrefabInstance = null;

    //Train healing & Damage
    protected HealingInfo TrainHealinginfo;
    protected DamageInfo TrainDamageinfo;
    
    
    private EventInstance _fixingSFXInstance;

    public UnityEvent OnObjectFixed;

    protected override void OnEnable()
    {
        base.OnEnable();

        BreakableObjectData.OnGameLost?.AddListener(OnGameLost);

        _health?.OnDeath.AddListener(BreakObject);
        _health?.OnFullyCured.AddListener(FullyRepairObject);

        DebugMenuSystem.Instance.RegisterObject(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        BreakableObjectData.OnGameLost?.RemoveListener(OnGameLost);

        _health?.OnDeath.RemoveListener(BreakObject);
        _health?.OnFullyCured.RemoveListener(FullyRepairObject);

        DebugMenuSystem.Instance.DeregisterObject(this);

        if (AudioInstanceHandler.CheckIfPlayingSFX(_fixingSFXInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_fixingSFXInstance);
        }
    }

    protected virtual void Start()
    {
        if (!_health.IsFullyCured)
            return;

        MaterialPlacedFill.SetActive(false);
        FixedFill.SetActive(false);
        BrokenFill.SetActive(false);
        InitialFill.SetActive(true);

        RepairStatus = RepairState.Fixed;

        TrainHealinginfo = new HealingInfo(BreakableObjectData.TrainHealAmount, null, gameObject, gameObject, HealType.Fixing);
        TrainDamageinfo = new DamageInfo(BreakableObjectData.TrainDamageAmount, null, gameObject, gameObject, DamageType.Object);

        if (_startBroken)
        {
            DamageInfo selfDamage = new DamageInfo(BreakableObjectData.Health, null, gameObject, gameObject, DamageType.Object);
            _health?.Damage(selfDamage);
        }
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        if (!canInteract)
            return;

        _warning.WarningOnInteract(interactor, RepairStatus);

        if (RepairStatus == RepairState.Fixed || !_health.CanBeHealed)
            return;

        HealingInfo healingInfo = new HealingInfo(BreakableObjectData.RepairAmountPerInitialInteraction, gameObject, interactor.gameObject, interactor.gameObject, HealType.Fixing);

        //On broken state
        if (RepairStatus == RepairState.Broken)
        {
            if (interactor.ObjectInHand == null || interactor.ObjectInHand.PickupableData != BreakableObjectData.RequireItem)
                return;

            //Use and placed the resource on the broken object
            interactor.TryCleanUp();

            ConsumeResource(healingInfo);
            Fixing(interactor, healingInfo);

            return;
        }

        //On Material Placed state
        if (RepairStatus == RepairState.MaterialPlaced)
        {
            Fixing(interactor, healingInfo);
        }
    }
    protected virtual void ConsumeResource(HealingInfo healingInfo)
    {
        BrokenFill.gameObject.SetActive(false);
        MaterialPlacedFill.gameObject.SetActive(true);
        
        RepairStatus = RepairState.MaterialPlaced;

    }
    protected virtual void Fixing(Interactor interactor, HealingInfo healingInfo) 
    {
        //Apply healing on initial input to fix faster on spamming the button
        _health?.Heal(healingInfo);
        
        if(_fixingVFXInstance == null)
            _fixingVFXInstance = Instantiate(_fixingVFXPrefab, VFXSpawnPoint.position, Quaternion.identity);

        StartCoroutine(FillOnHeld(interactor, BreakableObjectData.TimeToFinishRepairing));
    }
    protected virtual IEnumerator FillOnHeld(Interactor interactor, float repairTime)
    {
        //Spread the repair over a given time
        float healingRate = _health.MaxHealth / repairTime;

        _fixingSFXInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_fixingSFXInstance, TaskData.AudioData.FixingSFX);
        _fixingSFXInstance.set3DAttributes(transform.position.To3DAttributes());
        //Check if the input is being held
        while (interactInput > 0f)
        {
            //Check that the player is close to avoid continuing to interact when he is far away
            if (Vector3.Distance(interactor.transform.position, transform.position) > TaskData.ContinuedInteractionDistance)
                break;

            if (_fixingVFX != null || _fixingVFXInstance != null)
            {
                _fixingVFX.Play();
            }
            OnSetPlayerFixing?.Invoke(true);

            //Spread smoothly the repair per frame
            float repairProgress =+ healingRate * Time.deltaTime;

            HealingInfo healingInfo = new HealingInfo(repairProgress, gameObject, null, null, HealType.Fixing);
            _health?.Heal(healingInfo);
            yield return null;
        }

        AudioInstanceHandler.StopAndReleaseSFXInstance(_fixingSFXInstance);
        OnSetPlayerFixing?.Invoke(false);
        
        //Stop the vfx when the input is not longer pressed
        _fixingVFX.Stop();
        Destroy(_fixingVFXInstance);
    }

    protected virtual void FullyRepairObject(HealingInfo healingInfo)
    {
        _fixingVFX.Stop();
        Destroy(_fixingVFXInstance);

        _repairedVFXPrefabInstance = Instantiate(_repairedVFXPrefab, VFXSpawnPoint.position, VFXSpawnPoint.rotation);
        
        if (_repairedVFXPrefabInstance != null)
        {
            _repairedVFX = _repairedVFXPrefabInstance.GetComponent<VisualEffect>();
            if (_repairedVFX != null)
            {
                StartCoroutine(FadeOut(startAlpha, endAlpha, duration, _repairedVFX));
            }
        }

        MaterialPlacedFill.SetActive(false);
        FixedFill.SetActive(true);
        
        //Let know the Ai that the object was repair and heal the train
        BreakableObjectData.OnRepairObject?.Invoke(gameObject);

        BreakableObjectData.OnHealingTrain?.Invoke(TrainHealinginfo);

        OnObjectFixed.Invoke();

        RepairStatus = RepairState.Fixed;

        _health.ActivateInvulnerabilityDuringTime(BreakableObjectData.InvulnerabilityAfterRepaired);
    }
    
    protected IEnumerator FadeOut(float start, float end, float totalTime, VisualEffect steamVFX)
    {
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, elapsedTime / totalTime);
            steamVFX.SetFloat("AlphaValue", alpha);
            yield return null;
        }
        steamVFX.SetFloat("AlphaValue", endAlpha);
        Destroy(steamVFX.gameObject, 1f);
    }
    
    protected virtual void BreakObject(DamageInfo damageInfo)
    {
        InitialFill.SetActive(false);
        FixedFill.SetActive(false);
        BrokenFill.SetActive(true);

        //Let know the Ai that the object was broken and damage the train
        // Debug.Log("Calling event" + this.name);
        BreakableObjectData.OnBreakObject?.Invoke(gameObject);
        BreakableObjectData.OnDamageTrain?.Invoke(TrainDamageinfo);

        RepairStatus = RepairState.Broken;
    }

    public void TakeDamageFromBomb(Bomb bomb)
    {
        DamageInfo damageInfo = new DamageInfo(bomb.BombData.DamageToBreakableObjects, gameObject, bomb.gameObject, bomb.gameObject, DamageType.Bomb);
        _health?.Damage(damageInfo);
    }
}

public enum RepairState
{
    MaterialPlaced,
    Fixed,
    Broken
}