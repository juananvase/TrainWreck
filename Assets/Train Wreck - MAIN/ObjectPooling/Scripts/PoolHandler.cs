using UnityEngine;
using UnityEngine.Pool;

public class PoolHandler
{
    public LinkedPool<PooledObject> Pool { get; private set; }
    
    private Transform _parent;
    private Vector3 _hidePosition = new Vector3(0f, -1000f, 0f);
    private PooledObject _prefab;

    public PoolHandler(PooledObject prefab, Transform root)
    {
        _prefab = prefab;
        _parent = new GameObject($"{_prefab.name} Pool").transform;
        _parent.SetParent(root);
        // if(_prefab.LogCreation) Debug.Log($"{_prefab.name} has been created, size: {_prefab.PoolSize}");
        
        Pool = new LinkedPool<PooledObject>(OnCreateItem, OnGetItem, OnReturnItem, OnDestroyItem, true, _prefab.PoolSize);
    }

    private PooledObject OnCreateItem()
    {
        PooledObject instance = GameObject.Instantiate(_prefab, _hidePosition, Quaternion.identity, _parent);
        instance.Pool = Pool;
        return instance;
    }

    private void OnGetItem(PooledObject obj)
    {
        obj.gameObject.SetActive(true);
        obj.IsInPool = false;
    }

    private void OnReturnItem(PooledObject obj)
    {
        obj.IsInPool = true;
        obj.gameObject.SetActive(false);
        obj.transform.position = _hidePosition;
        obj.transform.SetParent(_parent);
    }

    private void OnDestroyItem(PooledObject obj)
    {
        GameObject.Destroy(obj.gameObject);
    }
}
