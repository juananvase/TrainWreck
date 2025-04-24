using System.Collections;
using CustomFMODFunctions;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using DebugMenu;
using EditorTools;
using FMODUnity;
using PrimeTween;
using Remaping;
using Unity.Cinemachine;
using UnityEngine.VFX;
using System;
using UnityEngine.Events;

public class Firebox : Task
{
    [field: SerializeField] public CameraDataSO CameraData{ get; private set; }
    [field: SerializeField] public CinemachineImpulseSource ImpulseSource { get; set; }
    public FireboxTaskDataSO FireboxData => TaskData as FireboxTaskDataSO;
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent), FoldoutGroup("Data")] private GUIFireboxWarning _warning;
    [SerializeField, FoldoutGroup("Data")] private BoolEventAsset _onSpawnFire;
    [SerializeField, FoldoutGroup("Data")] private BoolEventAsset _onLeverSwitch;
    [SerializeField, FoldoutGroup("Data")] private GameObject _door;
    [SerializeField, FoldoutGroup("Data")] private Material _fireboxBodyMaterial;


    [SerializeField, FoldoutGroup("SquashStrechAnimation")] private Vector3 _squashValue;
    [SerializeField, FoldoutGroup("SquashStrechAnimation")] private Vector3 _strechValue;
    [SerializeField, FoldoutGroup("SquashStrechAnimation")] private float _minAnimSpeedMultiplier;
    [SerializeField, FoldoutGroup("SquashStrechAnimation")] private float _maxAnimSpeedMultiplier;
    [SerializeField, FoldoutGroup("SquashStrechAnimation")] private AnimationCurve _squashStrechAnimCurve;

    [field: SerializeField, FoldoutGroup("VFX")] private VisualEffect CoalConsumedVFX { get; set; }
    [field: SerializeField, FoldoutGroup("VFX")] public VisualEffect FireBoxExplosionVFX { get; private set; }

    [field: SerializeField, FoldoutGroup("Events")] private FloatEventAsset _onReachingDistanceUpdated;

    public bool IsDoorOpen { get; private set; } = false;
    private bool _isFireboxEmpty => FireboxData.CurrentFuelLevel <= 0;
    private FMOD.Studio.EventInstance _burningSFX; //SFX loop for burning the fuel

    private Coroutine _fuelIncreaseCoroutine;
    private Coroutine _fuelDecreaseCoroutine;
    private Coroutine _fuelManagementCoroutine;

    public UnityEvent OnAddFuel;

    private void OnValidate()
    {
        if (ImpulseSource != null)
        {
            return;
        }
        ImpulseSource = GetComponent<CinemachineImpulseSource>();
        ImpulseSource.ImpulseDefinition = CameraData.ImpulseSourceBombExplosion;

        AssignVFX();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;

        FireboxData.OnGameLost?.AddListener(OnGameLost);
        FireboxData.OnWinLevel?.AddListener(OnWinLevel);

        _onLeverSwitch.OnInvoked.AddListener(SetDoorState);

        _onReachingDistanceUpdated.AddListener(UpdateTrainStationDistance);

        if (DebugMenuSystem.Instance != null)
        {
            DebugMenuSystem.Instance.RegisterObject(this);
        }

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        FireboxData.OnGameLost?.RemoveListener(OnGameLost);
        FireboxData.OnWinLevel?.RemoveListener(OnWinLevel);


        _onLeverSwitch.OnInvoked.RemoveListener(SetDoorState);

        _onReachingDistanceUpdated.RemoveListener(UpdateTrainStationDistance);

        //Stop Sound loop on change scenes (Levels or menus)
        StopSFX(_burningSFX);

        if (DebugMenuSystem.Instance != null)
        {
            DebugMenuSystem.Instance.DeregisterObject(this);
        }

    }

    private void OnWinLevel(bool value)
    {
        if(_fuelDecreaseCoroutine!=null)
            StopCoroutine(_fuelDecreaseCoroutine);
        if (_fuelManagementCoroutine != null)
            StopCoroutine(_fuelManagementCoroutine);
    }
    private void UpdateTrainStationDistance(float progress)
    {
        float fuelLevel = Mathf.Lerp(0, FireboxData.MediumThreshold, progress);
        FireboxData.OnFuelLevelUpdated?.Invoke(fuelLevel);
    }

    protected override void OnGameLost(bool isGameLost)
    {
        StopSFX(_burningSFX);
    }

    private void AssignVFX()
    {
        if (FireBoxExplosionVFX == null)
        {
            if (TryGetComponent(out VisualEffect fireboxExplosion))
            {
                FireBoxExplosionVFX = fireboxExplosion;
            }
        }

        if (CoalConsumedVFX == null)
        {
            if (TryGetComponent(out VisualEffect coalConsumedVFX))
            {
                CoalConsumedVFX = coalConsumedVFX;
            }
        }
        
        FireBoxExplosionVFX?.Stop();
        CoalConsumedVFX?.Stop();
    }
    private void Awake() => AssignVFX();   

    private void Start()
    {
        FireboxData.OnFuelLevelUpdated?.Invoke(FireboxData.InitialFuelLevel);

        //Start burning/consuming the fuel
        _fuelManagementCoroutine = StartCoroutine(GaugeLevelManagerCoroutine());

        StartCoroutine(FeedbackCoroutine());

        StartCoroutine(FireboxSquashStrechAnimation());

        IsDoorOpen = false;
    }


    private IEnumerator FeedbackCoroutine()
    {
        while (true)
        {
            ManagePlayStopBurningSFX();

            //Increase strength of the sfx base on fuel level
            _burningSFX.setParameterByName("FireboxFuelLevel", FireboxData.CurrentFuelLevel);

            //Color feedback on Firebox, base on the state
            _fireboxBodyMaterial.color = Remap.ColorRemap(FireboxData.MediumThreshold, FireboxData.OverloadThreshold, FireboxData.NormalFireboxColor, FireboxData.heatedFireboxColor, FireboxData.CurrentFuelLevel);

            yield return null;
        }
    }

    private IEnumerator FireboxSquashStrechAnimation() 
    {
        while (true) 
        {
            float timer = 0f;
            float frequency = 0f;

            if (frequency <= 0)
            {
                frequency = 0.1f;
            }

            while (timer < 1)
            {
                frequency = Remap.FloatRemap(FireboxData.LowThreshold, FireboxData.OverloadThreshold, _minAnimSpeedMultiplier, _maxAnimSpeedMultiplier, FireboxData.CurrentFuelLevel);
                timer += Time.deltaTime * frequency;
                transform.localScale = Vector3.Lerp(_squashValue, _strechValue, _squashStrechAnimCurve.Evaluate(timer));

                yield return null;
            }
            yield return null;
        }
    }

    private void SetDoorState(bool isOpen)
    {
        IsDoorOpen = isOpen;
        if (isOpen) OpenDoor(); else CloseDoor();
    }
    private void CloseDoor()
    {
        //Animation of door closing
        Tween.LocalRotation(_door.transform, endValue: new Vector3(0, 0, 110), duration: 0.3f).OnComplete(() => IsDoorOpen = false, warnIfTargetDestroyed: false);
    }
    private void OpenDoor()
    {
        CheckOverClockFire();

        //Animation of door opening
        Tween.LocalRotation(_door.transform, endValue: new Vector3(0, 0, 0), duration: 0.3f).OnComplete(() => IsDoorOpen = true, warnIfTargetDestroyed: false);
    }

    /// <summary>
    /// Overclock happen when the firebox fuel level is high enough and someone open the door
    /// </summary>
    private void CheckOverClockFire()
    {
        if (FireboxData.CurrentFuelLevel >= FireboxData.OverclockThreshold && FireboxData.CurrentFuelLevel <= FireboxData.OverloadThreshold)
        {
            ExplodeFirebox();
        }
    }

    /// <summary>
    /// Overload happend when too much fuel is put into the firebox
    /// </summary>
    private void CheckOverLoadFire()
    {
        if (FireboxData.CurrentFuelLevel >= FireboxData.OverloadThreshold)
        {
            ExplodeFirebox();
        }
    }
    private void ExplodeFirebox() 
    {
        _onSpawnFire?.Invoke(true);

        PlaySFX();
        //Warning indicating the fire extinguisher
        _warning.WarningOnExplosion();

        //Spawn explotion vfx feedback
        FireBoxExplosionVFX.Play();
        
        if (ImpulseSource != null)
            ImpulseSource.GenerateImpulse(); // camera shake
        
    }

    private void PlaySFX()
    {
        if (TaskData.AudioData != null)
        {  
            RuntimeManager.PlayOneShot(TaskData.AudioData.FireboxExplosionSFX, transform.position);
        }
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        if (!canInteract)
            return;

        //Check if a warning for missing a DecalProjector is necessary
        _warning.WarningOnInteract(interactor);

        PickupableContainer container = interactor.ObjectInHand as PickupableContainer;

        if (container != null && IsDoorOpen == true)
        {
            if (container.ItemInContainer == null || container.ItemInContainer.PickupableData != FireboxData.RequireItem)
                return;

            container.EmptyContainer();

            IncreaseGaugeLevel(FireboxData.FuelAdded);

            PlaySFX(FireboxData.AudioData.FuelingFirebox);

            OnAddFuel.Invoke();
        }
    }

    [Button("Add Fuel")]
    private void IncreaseGaugeLevel(float coalAddedAmount)
    {
        if (_fuelIncreaseCoroutine != null)
        {
            StopCoroutine(_fuelIncreaseCoroutine);
        }

        if (_fuelDecreaseCoroutine != null)
        {
            StopCoroutine(_fuelDecreaseCoroutine);
            _fuelDecreaseCoroutine = null;
        }

        float targetCoalAmount = Mathf.Clamp(FireboxData.CurrentFuelLevel + coalAddedAmount, 0, FireboxData.MaxFuelLevel);
        _fuelIncreaseCoroutine = StartCoroutine(AddCoalRoutine(targetCoalAmount));

        CoalConsumedVFX.Play();
        
    }

    private IEnumerator AddCoalRoutine(float targetCoalAmount)
    {
        float currentFuelLevel = FireboxData.CurrentFuelLevel;
        float timer = 0f;

        while (!Mathf.Approximately(currentFuelLevel,targetCoalAmount))
        {
            timer += Time.fixedDeltaTime;
            float duration = timer / 2;
            currentFuelLevel = Mathf.Lerp(currentFuelLevel, targetCoalAmount, duration);

            FireboxData.OnFuelLevelUpdated?.Invoke(currentFuelLevel);
            CheckOverLoadFire();
            yield return null;
        }
        _fuelIncreaseCoroutine = null;
    }


    private void DecreaseGaugeLevel(float coalConsumedAmount)
    {
        if (_fuelDecreaseCoroutine != null)
        {
            StopCoroutine(_fuelDecreaseCoroutine);
        }

        float targetCoalAmount = Mathf.Clamp(FireboxData.CurrentFuelLevel - coalConsumedAmount, 0, FireboxData.MaxFuelLevel);
        _fuelDecreaseCoroutine = StartCoroutine(ConsumeCoalRoutine(targetCoalAmount));
    }

    private IEnumerator ConsumeCoalRoutine(float targetCoalAmount)
    {
        float unitConsimptionTime = 0.1f;
        float unitConsimptionAmount = (FireboxData.FuelConsumptionAmount * unitConsimptionTime) / FireboxData.FuelConsumptionRate;

        while (!Mathf.Approximately(FireboxData.CurrentFuelLevel, targetCoalAmount))
        {
            float targetAmountPerUnit = Mathf.Clamp(FireboxData.CurrentFuelLevel - unitConsimptionAmount, 0, FireboxData.MaxFuelLevel);
            float timer = 0;

            while (timer < unitConsimptionTime) 
            {
                timer += Time.deltaTime;
                float progress = timer / unitConsimptionTime;

                float currentFuelLevel = Mathf.Lerp(FireboxData.CurrentFuelLevel, targetAmountPerUnit, progress);
                FireboxData.OnFuelLevelUpdated?.Invoke(currentFuelLevel);
                yield return null;
            }

            yield return null;
        }

        _fuelDecreaseCoroutine = null;
    }

    private IEnumerator GaugeLevelManagerCoroutine()
    {
        while (true)
        {
            //On empty warn the player that firebox needs coal
            _warning.WarningOnEmptyFuel(_isFireboxEmpty);

            if (_fuelIncreaseCoroutine == null && _fuelDecreaseCoroutine == null)
            {
                DecreaseGaugeLevel(FireboxData.FuelConsumptionAmount);
            }

            yield return null;
        }
    }
    private void ManagePlayStopBurningSFX()
    {
        if (!_isFireboxEmpty && !IsPlayingSFX(_burningSFX))
        {
            _burningSFX = AudioInstanceHandler.CreateAndPlaySFXInstance(_burningSFX, TaskData.AudioData.FireBoxBurn);
            _burningSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            _burningSFX.setParameterByName("FireboxFuelLevel", FireboxData.CurrentFuelLevel);
        }

        if (_isFireboxEmpty)
        {
            if (AudioInstanceHandler.CheckIfPlayingSFX(_burningSFX))
            {
                AudioInstanceHandler.StopAndReleaseSFXInstance(_burningSFX);
            }
        }
    }

    //DEBUG
    [DebugCommand("Add coal")]
    public void DebugAddCoal()
    {
        IncreaseGaugeLevel(10f);
    }

    [DebugCommand("Remove coal")]
    public void DebugRemoveCoal()
    {
        DecreaseGaugeLevel(10f);
    }
}