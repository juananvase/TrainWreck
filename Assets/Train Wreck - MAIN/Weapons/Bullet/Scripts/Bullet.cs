using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : PooledObject
{
    [field: SerializeField] public BulletDataSO BulletData { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public GameObject BulletTrailVFX { get; private set; }

    private bool _isInit = false;
    private Vector3 _spawnPosition;

    private void OnValidate()
    {
        if(Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();
        if(BulletTrailVFX == null) BulletTrailVFX = GetComponent<GameObject>();
        Rigidbody.useGravity = false;
    }

    private void Update()
    {
        if (_isInit == false)
        {
            return;
        }
        
        float distanceTravelled = Vector3.Distance(transform.position, _spawnPosition);
        if (distanceTravelled > BulletData.Range)
        {
            if(distanceTravelled > 500f) Debug.LogError($"Bullet distance too far: {distanceTravelled}");
            Cleanup();
        }
    }

    public void ShootBullet(Vector3 spawnPosition)
    {
        Rigidbody.linearVelocity = this.transform.forward * BulletData.BulletVelocity;
        _spawnPosition = spawnPosition;
        transform.position = spawnPosition;
        Rigidbody.position = spawnPosition;
        Rigidbody.rotation = this.transform.rotation;
        _isInit = true;
    }

    private void Cleanup()
    {
        _isInit = false;
        ReturnToPool();
        ResetRigidbody();
    }
    
    private void ResetRigidbody()
    {
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            DamageInfo damageInfo = new DamageInfo(BulletData.Damage, collision.gameObject, gameObject, gameObject, DamageType.Gun);
            health?.Damage(damageInfo);
        }
        PlayVFX();
        ReturnToPool();
    }

    private void PlayVFX() 
    {
        PoolSystem.Instance.Get(BulletData.HitVFX, transform.position, transform.rotation);
    }
}