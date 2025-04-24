using Sirenix.OdinInspector;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [SerializeField, InlineEditor] private MountedGunDataSO _gunData;
    [SerializeField] private Transform _rayStartPoint;
    [SerializeField] private float _newSensitivity;
    [SerializeField] private float _rayMaxDistance = 10f;

    private float _originalSensitivity => _gunData.RotationSpeed;

    private void Update()
    {
        if (EnemiesInRange())
        {
            _gunData.RotationSpeed = _newSensitivity;
        }
        else
        {
            _gunData.RotationSpeed = _originalSensitivity;
        }
    }

    private void OnDisable()
    {
        _gunData.RotationSpeed = _originalSensitivity;
    }

    private bool EnemiesInRange()
    {
        RaycastHit[] hits = new RaycastHit[10];
        int size = Physics.RaycastNonAlloc(_rayStartPoint.position, transform.forward, hits, _rayMaxDistance);
        Debug.DrawRay(_rayStartPoint.position, transform.forward * _rayMaxDistance, Color.red);

        if (size == 0)
        {
            return false;
        }

        foreach (var hit in hits)
        {
            return hit.collider.TryGetComponent(out EnemyController _);
        }
        return false;
    }
    
}
