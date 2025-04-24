using UnityEngine;

public class EnemyWheelRotator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private GameObject[] _wheels;
    [Header("Speed Settings")]
    [SerializeField] private float _slow = 2f;
    [SerializeField] private float _normal = 2f;
    [SerializeField] private float _fast = 2f;
    [SerializeField] private float _superFast = 2f;
    [SerializeField] private float _speedMultiplier = 2f;
    
    private void Update()
    {
        switch (_trainData.CurrentTrainSpeed)
        {
            case 0:
                SetWheelRotationSpeed(_slow);
                break;
            case < 20:
                SetWheelRotationSpeed(_normal);
                break;
            case > 20 and < 100:
                SetWheelRotationSpeed(_fast);
                break;
            case > 100:
                SetWheelRotationSpeed(_superFast);
                break;
        }
    }

    private void SetWheelRotationSpeed(float speed)
    {
        float newDir = 1f * (speed * _speedMultiplier * Time.deltaTime);
        foreach (var wheel in _wheels)
        {
            var smoothRotation = Mathf.Lerp(wheel.transform.position.z, newDir, 1);
            wheel.transform.Rotate(new Vector3(0f, 0f, smoothRotation));
        }
    }
}
