using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using CustomFMODFunctions;
using FMODUnity;
using GameEvents;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class Bomb : Pickupable
{
    [field: SerializeField] public CinemachineImpulseSource ImpulseSource { get; set; }
    [field: SerializeField] public CameraDataSO CameraData { get; set; }
    public BombDataSO BombData => PickupableData as BombDataSO; //Cast to use PickupableData data as BombDataSO

    [SerializeField, FoldoutGroup("Bomb Animation")] private Animator _animator;
    [SerializeField, FoldoutGroup("Bomb Animation")] private AnimationCurve _flickerSpeedCurve;
    [SerializeField] private GameObjectEventAsset _onBombExplodeInHand;
    [SerializeField] private BoolEventAsset _onSpawnFire;
    [SerializeField] private GameObject _bombRadiusVFX;

    private EventInstance _beepInstance;
    private IBombDamageable _targetObj;
    private bool _isInsideTrain;


    private void OnValidate()
    {
        if (ImpulseSource != null)
        {
            return;
        }
        ImpulseSource = GetComponent<CinemachineImpulseSource>();
        ImpulseSource.ImpulseDefinition = CameraData.ImpulseSourceBombExplosion;
    }

    private void OnEnable()
    {
        StartCoroutine(TriggerExplosionSequence());
        BombData.OnGameLost?.AddListener(OnGameLost);
        
        if(_bombRadiusVFX != null)
            _bombRadiusVFX.SetActive(false);
    }
    private void OnDisable()
    {
        BombData.OnGameLost?.RemoveListener(OnGameLost);
        
        //stop SFX instances 
        if (IsPlayingInstance(_beepInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_beepInstance);
        }
        if(_bombRadiusVFX != null)
            _bombRadiusVFX.SetActive(false);
    }

    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.BombPickupSFX, pickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.DropBombSFX, transform.position);
    }

    private void OnGameLost(bool value)
    {
        //stop SFX instances when game lost
        if (IsPlayingInstance(_beepInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_beepInstance);
        }
    }

    private bool IsPlayingInstance(EventInstance instance)
    {
        return AudioInstanceHandler.CheckIfPlayingSFX(instance);
    }

    private IEnumerator TriggerExplosionSequence()
    {
        // Start bomb timer
        float countdownTimer = 0f;
        while (countdownTimer < BombData.ExplosionDelay)
        {
            if (_isInsideTrain)
            {
                _beepInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_beepInstance, AudioData.BombTickingSFX);
                _beepInstance.set3DAttributes(transform.position.To3DAttributes());
                if (_bombRadiusVFX != null)
                {
                    _bombRadiusVFX.SetActive(true);
                }
            }
            
            float tickRate = 1f / BombData.ExplosionDelay;
            _beepInstance.setParameterByName("TickingRate", tickRate); 
            countdownTimer += Time.deltaTime;

            //Flicker Animation
            float normalizedTime = Mathf.Clamp01(countdownTimer / BombData.ExplosionDelay);
            float flickerSpeed = _flickerSpeedCurve.Evaluate(normalizedTime);
            _animator.SetFloat("FlickeringMultiplier", flickerSpeed);

            // Wait for the next frame
            yield return null;
        }

        // After the countdown is complete, proceed with the explosion sequence
        if (AudioInstanceHandler.CheckIfPlayingSFX(_beepInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_beepInstance); // stop Beeping sfx
        }
        // play Explosion sfx
        RuntimeManager.PlayOneShot(AudioData.BombExplosionSFX);

        _bombRadiusVFX.SetActive(false);
        ApplyExplosionDamage();

        if (IsBeingHeld)
        {
            _onBombExplodeInHand?.Invoke(transform.parent.gameObject);

            GameObject instance = Instantiate(BombData.ExplosionOnHanVFX, transform.position, transform.rotation);
            Destroy(instance, 2f);
        }
        else 
        {
            GameObject instance = Instantiate(BombData.ExplosionOnFloorVFX, transform.position, transform.rotation);
            Destroy(instance, 2f);
        }
        
        if (ImpulseSource)
            ImpulseSource.GenerateImpulse();

        ReturnToPool();
        SetValuesToDefault();
    }

    private void ApplyExplosionDamage()
    {
        int maxColliders = 30;
        Collider[] affectedObjects = new Collider[maxColliders];
        int size = Physics.OverlapSphereNonAlloc(transform.position, BombData.Range, affectedObjects);

        if (size == 0)
            return;

        HashSet<IBombDamageable> damagedObjects = new HashSet<IBombDamageable>();

        foreach (Collider affectedObject in affectedObjects)
        {
            if (affectedObject == null)
                continue;

            IBombDamageable obj1 = affectedObject.GetComponent<IBombDamageable>();
            IBombDamageable obj2 = affectedObject.transform.parent?.GetComponent<IBombDamageable>();
            IBombDamageable obj3 = affectedObject.transform.root?.GetComponent<IBombDamageable>();
            
           
            if(obj1 != null)
                _targetObj = obj1;
            else if(obj2 != null)
                _targetObj = obj2;
            else
                _targetObj = obj3;

            if (_targetObj != null && damagedObjects.Contains(_targetObj) == false)
            {
                damagedObjects.Add(_targetObj);
                SendDamageToObject(_targetObj);
            }

            if (affectedObject.TryGetComponent(out Firebox firebox))
            {
                _onSpawnFire.Invoke(true);
                firebox.FireBoxExplosionVFX.Play();
            }
        }
    }
    private void SendDamageToObject(IBombDamageable affectedObject) 
    {
        affectedObject.TakeDamageFromBomb(this);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.up * 1f,  Color.red);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isInsideTrain = true;
        }
    }
}
