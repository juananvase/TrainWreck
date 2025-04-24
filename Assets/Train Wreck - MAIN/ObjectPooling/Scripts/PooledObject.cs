using UnityEngine;
using UnityEngine.Pool;

public class PooledObject : MonoBehaviour
{
    [field: Header("ObjectPooling")]
    [field: SerializeField] public int PoolSize { get; set; } = 10;
    [field: SerializeField] public bool LogCreation { get; set; } = true;
    
    public bool IsInPool { get; set; }
    public LinkedPool<PooledObject> Pool { get; set; }

    protected void ReturnToPool()
    {
        if (IsInPool)
        {
            return;
        }
        Pool.Release(this);
    }
}
 