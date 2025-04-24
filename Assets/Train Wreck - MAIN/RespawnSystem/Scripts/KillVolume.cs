using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private Transform _respawnPoint;
    
    private void Start()
    {
        if (_respawnPoint == null || GameManager.Instance != null)
        {
            _respawnPoint = GameManager.Instance.RespawnPoint;
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.TryGetComponent(out IRespawnable _))
        {
            ResetObject(obj);
        }
    }

    private void ResetObject(Collider obj)
    {
        if (obj.attachedRigidbody != null)
        {
            obj.attachedRigidbody.position = _respawnPoint.position;
        }
    }
}
