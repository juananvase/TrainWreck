using FMODUnity;
using UnityEngine;

public class Foam : PooledObject
{
    [field: SerializeField] private Rigidbody Rigidbody;
    [field: SerializeField] private FoamDataSO _foamDataSO;
 
    private void OnValidate()
    {
        if(Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float scale = Random.Range(0.1f, 0.5f);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out Fire fire))
        {
            ExtinguishFire(fire);
        }
        else if(collider.gameObject.TryGetComponent(out FireOnPlayerVFX _))
        {
            PlayerStateMachine player = collider.gameObject.GetComponentInParent<PlayerStateMachine>();
            if (player != null)
            {
                player.InitDestroyFireOnPlayer();
            }
        }
    }

    private void ExtinguishFire(Fire fire)
    {
        if (fire == null)
        {
            return;
        }
        if (fire.TryGetComponent(out Health health))
        {
            DamageInfo damageInfo = new DamageInfo(_foamDataSO.DamageToFireAmount, gameObject, gameObject, gameObject, DamageType.Foam);
            health.Damage(damageInfo);
        }
    }

    public void ShootFoam(Transform spawnPosition)
    {
        Rigidbody.linearVelocity = spawnPosition.forward * _foamDataSO.FoamSpawnForce; 
    }

    public void CleanUp()
    {
        ReturnToPool();
    }
}
