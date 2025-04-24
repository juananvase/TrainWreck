using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class FireOnPlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject _fireOnPlayerVFXPrefab;
    [SerializeField] private VisualEffect _fireVFX;
        
    private GameObject _fireInstance;
    private PlayerStateMachine _player;
    private Vector3 _randomDirection;
    private float _timeSinceLastDirectionChange = 0f;

    private bool _fireDestroyed;

    private void Start()
    {
        if (_player == null)
        {
            _player = GetComponentInParent<PlayerStateMachine>();
        }
    }
    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        if (_player.IsOnFire && _player.MoveInput == Vector2.zero)
        {
            _timeSinceLastDirectionChange += Time.deltaTime;
            if (_timeSinceLastDirectionChange > _player.CharacterDataSO.RandomDirectionChangeInterval)
            {
                _randomDirection = GetRandomDirection();
                _timeSinceLastDirectionChange = 0f;
            }
            Vector3 movement = _randomDirection * (Time.deltaTime * _player.CharacterDataSO.OnFireSpeed);
            MoveLikeRoomba(movement);
        }
    }
    
    private void MoveLikeRoomba(Vector3 movement)
    {
        Vector3 newPosition = _player.Rigidbody.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, _player.CharacterDataSO.MinBounds.x, _player.CharacterDataSO.MaxBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, _player.CharacterDataSO.MinBounds.z, _player.CharacterDataSO.MaxBounds.z);
        
        _player.Rigidbody.MovePosition(newPosition);
    }

    private Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
    }
}
