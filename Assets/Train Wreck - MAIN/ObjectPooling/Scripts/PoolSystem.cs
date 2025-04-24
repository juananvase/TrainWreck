using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    private static PoolSystem _instance;
    public static PoolSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                // Debug.Log("PoolSystem Instantiated");
                _instance = new GameObject("PoolSystem").AddComponent<PoolSystem>();
                _instance.PoolHandlers = new Dictionary<PooledObject, PoolHandler>();
            }

            return _instance;
        }
        set => _instance = value;
    }

    private Dictionary<PooledObject, PoolHandler> PoolHandlers { get; set; }

    public PooledObject Get(PooledObject prefab, Vector3 position, Quaternion rotation)
    {
        return Get(prefab, position, rotation, null);
    }
    
    public PooledObject Get(PooledObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!PoolHandlers.ContainsKey(prefab))
        {
            PoolHandler poolHandler = new PoolHandler(prefab, transform);
            PoolHandlers.Add(prefab, poolHandler);
        }

        PooledObject pooled = PoolHandlers[prefab].Pool.Get();
        pooled.transform.position = position;
        pooled.transform.rotation = rotation;
        pooled.transform.SetParent(parent);
        return pooled;
    }
}
