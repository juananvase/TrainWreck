using CustomFMODFunctions;
using FMOD.Studio;
using FMODUnity;
using GameEvents;
using UnityEngine;

public class Fire : FireBase
{
    [SerializeField] private Health _health;
    [SerializeField] private FireOnPlayerVFX _fireOnPlayerVFX;
    [SerializeField] private BoolEventAsset OnAllFiresDestroyed;

    private Health _trainsHealth;
    private EventInstance _fireAlarmInstance;

    protected override void Start()
    {
        base.Start();
        CheckForDamageableObjects();
        PlayFireAlarm();
    }

    private void OnEnable()
    {
        _health?.OnDeath.AddListener(DestroyFire);
    }

    private void OnDisable()
    {
        _health?.OnDeath.RemoveListener(DestroyFire);
        StopSFX();
    }

    private void CheckForDamageableObjects()
    {
        _trainsHealth = GetTrainsHealth();
        if (_trainsHealth != null)
        {
            DealDamage(_trainsHealth);
        }
    }

    private void DealDamage(Health damageableObjects)
    {
        DamageInfo damageInfo = new DamageInfo(FireData.DamageAmount, gameObject, gameObject, gameObject, DamageType.Fire);
        damageableObjects?.Damage(damageInfo);
    }

    private void HealDamagedObjects(Health damageableObjects)
    {
        HealingInfo healingInfo = new HealingInfo(FireData.HealDamagedAmount, gameObject, gameObject, gameObject, HealType.Fixing);
        damageableObjects?.Heal(healingInfo);
    }

    private void DestroyFire(DamageInfo damageInfo)
    {
        if (_trainsHealth != null)
        {
            HealDamagedObjects(_trainsHealth);
        }
        RemoveFromList(gameObject);
        StopSFX();
        Destroy(gameObject);
        if (!this.gameObject.activeInHierarchy)
        {
            OnAllFiresDestroyed?.Invoke(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (!player.IsOnFire)
            { 
                player.InitFireOnPlayer();
            }
        }
    }
    
    private Health GetTrainsHealth()
    {
        Collider[] results = new Collider[10];
        int hits = Physics.OverlapSphereNonAlloc(transform.position, FireData.NearbyObjectRadius, results);
        if(hits == 0) return null;

        for (int i = 0; i < hits; i++)
        {
            if (results[i].GetComponentInParent<TrainBase>())
            {
                Health trainHealth = results[i].GetComponentInParent<Health>();
                if(trainHealth != null)
                {
                    return trainHealth;
                }
            }
        }
        return null;
    }
    
    private void PlayFireAlarm()
    {
        if (AudioInstanceHandler.CheckIfPlayingSFX(_fireAlarmInstance) == false)
        {
            _fireAlarmInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_fireAlarmInstance, audioData.FireAlarmSFX);
            _fireAlarmInstance.set3DAttributes(transform.position.To3DAttributes());
        }
    }
    
    private void StopSFX()
    {
        if (AudioInstanceHandler.CheckIfPlayingSFX(_fireAlarmInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_fireAlarmInstance);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, FireData.NearbyObjectRadius);
        Gizmos.color = Color.red;
    }
}
