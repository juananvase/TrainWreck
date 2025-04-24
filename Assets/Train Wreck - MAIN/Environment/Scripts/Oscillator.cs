using System.Collections;
using GameEvents;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] protected TrainDataSO _trainData;
    [SerializeField] protected Transform _endLoc;
    [SerializeField] private float _resetDelay;
    [SerializeField] private float _speedMultiplier;
    [SerializeField] private FloatEventAsset OnTrainSpeedUpdated;

    protected Vector3 _startPos;
    protected bool _isMoving;

    private void Start()
    {
        _startPos = this.transform.position;        
    }

    private void OnEnable()
    {
        OnTrainSpeedUpdated.AddListener(GetTrainSpeed);
    }

    private void OnDisable()
    {
        OnTrainSpeedUpdated.RemoveListener(GetTrainSpeed);
    }
    
    private void GetTrainSpeed(float speed)
    {
        if(speed > 0 && !_isMoving)
        {
            StartCoroutine(InitOscillation());
        }
    }
    
    protected virtual IEnumerator InitOscillation()
    {
        _isMoving = true;
        
        yield return MoveToPosition(_endLoc.position);
        yield return new WaitForSeconds(_resetDelay);
        transform.position = _startPos;        
        
        _isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 pos)
    {
        while (Mathf.Abs(pos.x - transform.position.x) > 0.1f)
        { 
            float step = Time.deltaTime * _trainData.CurrentTrainSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.x, transform.position.y, transform.position.z), step * _speedMultiplier);
            yield return null;
        }
    }
}
