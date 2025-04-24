using GameEvents;
using UnityEngine;

public class TerrainMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] _grounds;
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private Vector2 _startEndPos = new Vector2(-140f, 0f);
    [SerializeField] private FloatEventAsset OnFuelUpdated;
    [SerializeField] private float _speed;
    private float _newSpeed;

    private void OnEnable()
    {
        OnFuelUpdated.AddListener(GetFuelLevel);
    }

    private void OnDisable()
    {
        OnFuelUpdated.RemoveListener(GetFuelLevel);
    }

    private void GetFuelLevel(float fuelLevel)
    {
        float elapsedTime = 0f;
        float currentSpeed = 0;
        while (elapsedTime <= fuelLevel)
        {
            elapsedTime += Time.deltaTime;
            _newSpeed = Mathf.Lerp(currentSpeed / 100f, fuelLevel, elapsedTime/ 1f);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _grounds.Length; i++)
        {
            GameObject ground = _grounds[i];
            float fraction = (Time.time * _newSpeed + 1f/_grounds.Length * i) % 1f;
            float xPos = Mathf.Lerp(_startEndPos.x, _startEndPos.y, fraction);
            Vector3 pos = ground.transform.position;
            pos.x = xPos;
            ground.transform.position = pos;
        }
    }
}
