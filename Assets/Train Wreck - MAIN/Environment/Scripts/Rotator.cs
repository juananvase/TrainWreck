using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speedMultiplier;
    
    private float rotatorSpeed => _trainData.CurrentTrainSpeed;

    private void Start()
    {
        StartCoroutine(RotateGORoutine());
    }

    private IEnumerator RotateGORoutine()
    {
        while (true)
        {
            if (_trainData != null && _trainData.CurrentTrainSpeed > 0)
            {
                Vector3 newDir = _direction * (rotatorSpeed * _speedMultiplier * Time.deltaTime);
                foreach (var obj in _gameObjects)
                {
                    var smoothRotation = Vector3.Lerp(obj.transform.position, newDir, 1);
                    obj.transform.Rotate(smoothRotation);
                }
            }
            yield return null;
        }
    }
}
