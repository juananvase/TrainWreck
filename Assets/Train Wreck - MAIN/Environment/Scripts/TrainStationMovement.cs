using System.Collections;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TrainStationMovement : MonoBehaviour
{
    [SerializeField] protected TrainDataSO _trainData;
    [SerializeField] protected Transform _startLoc;
    [SerializeField] protected Transform _endLoc;

    [SerializeField] private BoolEventAsset _spawnEnemies;
    [SerializeField] private BoolEventAsset _onLevelWin;
    [SerializeField] private FloatEventAsset _onReachingDistanceUpdated;

    private Coroutine _resetPosition = null;

    protected Vector3 _startPos;
    protected bool _isMoving;

    private void OnEnable()
    {
        _onLevelWin.AddListener(ResetPosition);
    }

    private void OnDisable()
    {
        _onLevelWin.RemoveListener(ResetPosition);
    }

    private void Start()
    {
        _startPos = this.transform.position;
    }

    private void Update()
    {
        if (_trainData.CurrentTrainSpeed > 0 && !_isMoving)
        {
            StartCoroutine(MoveToEndLocation());
        }
    }

    [Button(Icon = SdfIconType.ToggleOff)]
    private void ResetPosition(bool value)
    {
        if (_resetPosition == null)
        {
            StopAllCoroutines();
            _resetPosition = StartCoroutine(MoveToStartPosition(_startPos));
        }
    }

    private IEnumerator MoveToStartPosition(Vector3 pos)
    {
        float distance = Mathf.Abs(pos.x - transform.position.x);
        while (Mathf.Abs(pos.x - transform.position.x) > 0.1f)
        {
            float step = Time.deltaTime * _trainData.CurrentTrainSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.x, transform.position.y, transform.position.z), step);
            _onReachingDistanceUpdated?.Invoke(Mathf.Abs(pos.x - transform.position.x) / distance);
            yield return null;
        }
    }

    private IEnumerator MoveToEndLocation()
    {
        _isMoving = true;

        while (Mathf.Abs(_endLoc.position.x - transform.position.x) > 0.1f)
        {
            float step = Time.deltaTime * _trainData.CurrentTrainSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_endLoc.position.x, transform.position.y, transform.position.z), step);
            yield return null;
        }

        transform.position = _startLoc.position;
        _spawnEnemies?.Invoke(true);
        yield return null;
    }
}
