using UnityEngine;

public class TrainWheelRotation : MonoBehaviour
{
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _trainSpeedDividend = 50f;
    
    private Vector2 _position;
    private string _trainSpeedParameter = "TrainSpeed";
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(_trainData == null)
            return;

        if (_trainData.CurrentTrainSpeed > 0)
        {
            float newSpeed = _trainData.CurrentTrainSpeed / _trainSpeedDividend; 
            _animator.SetFloat(_trainSpeedParameter, newSpeed);
        }
    }
}
