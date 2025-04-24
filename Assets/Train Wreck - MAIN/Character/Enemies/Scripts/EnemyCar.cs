using GameEvents;
using Sirenix.OdinInspector;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyCar : MonoBehaviour
{
    //Data
    [SerializeField] private EnemyCarSO _carData;
    [SerializeField] private TrainDataSO _trainData;

    // private fields
    private float _sineTime;
    protected IEnumerator _currentMovement;
    protected IEnumerator _previousMovement;
    [ShowInInspector] public string _currentStateName;
    private bool _isShaking = false;
    private string _previousStateName;

    private bool _levelCompleted = false;

    [SerializeField] private VisualEffect _carDustVFX;
    [SerializeField] private VisualEffect _carTrailVFX;

    [SerializeField] public BoolEventAsset OnLevelComplete;

    private void OnEnable()
    {
        OnLevelComplete.AddListener(CompletedLevelRoutine);
    }
    private void OnDisable()
    {
        OnLevelComplete.RemoveListener(CompletedLevelRoutine);
    }

    private void Start()
    {
        StartCoroutine(DrivingMovementRoutine());
    }

    public IEnumerator DrivingMovementRoutine()
    {
        while (true)
        {
            _sineTime += Time.deltaTime * Random.Range(_carData.DrivingFrequency.x, _carData.DrivingFrequency.y);
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            float noise = Mathf.PerlinNoise(_sineTime * 0.1f, 0f) * 2f - 1f;

            float sineValue = Mathf.Sin(_sineTime) + noise;
            Vector3 offset = forward * (sineValue * Random.Range(_carData.DrivingMagnitude.x, _carData.DrivingMagnitude.y));
            transform.position += offset * Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator AttackMovementRoutine()
    {
        float perlinOffset = Random.Range(_carData.PerlinOffset.x, _carData.PerlinOffset.y);
        float remappedPerlin;
        float xOffset = 0;
        float zOffset = 0;

        while (true)
        {
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            float perlin = Mathf.PerlinNoise(Time.time * _carData.PerlinSpeed, perlinOffset);
            if (perlin <= 0) 
            {
                remappedPerlin = perlin * 2f - 1f;
                xOffset = remappedPerlin * 2f;
                zOffset = Mathf.Sin(_sineTime) * Mathf.Cos(_sineTime) * Random.Range(_carData.InfiniteMagnitude.x, _carData.InfiniteMagnitude.y);
                Vector3 attackOffset = (right * xOffset) + (forward * zOffset);
                transform.position += attackOffset * Time.deltaTime;
            }
            else
            {
                yield return null;
            }
            yield return null;
        }
    }

    private IEnumerator ShakeEffectRoutine()
    {
        _isShaking = true;
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;

        while (elapsedTime < _carData.ShakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float shakeAmount = Mathf.Sin(elapsedTime * 50) * 0.1f;
            transform.position = originalPosition + transform.right * shakeAmount;
            yield return null;
        }

        transform.position = originalPosition;
        _isShaking = false;
        if (_previousStateName == "DrivingMovement")
        {
            ChangeState(DrivingMovementRoutine());
        }
        else if (_previousStateName == "AttackMovement")
        {
            ChangeState(AttackMovementRoutine());
        }

    }

    public void ChangeState(IEnumerator newState)
    {
        if (!_levelCompleted)
        {
            string newStateName = newState.ToString();
            if (_currentStateName != "HitMovement")
            {
                _currentStateName = _previousStateName;
            }

            if (_currentMovement != null)
            {
                StopCoroutine(_currentMovement);
            }

            _currentMovement = newState;
            _currentStateName = _currentMovement.ToString();
            StartCoroutine(_currentMovement);
        }
        else if (_levelCompleted)
        {
            StopCoroutine(_currentMovement);
        }
        _currentMovement = newState;
        _currentStateName = _currentMovement.ToString();
        StartCoroutine(_currentMovement);
    }

    public void HitMovement()
    {
        if (_isShaking) return;

        ChangeState(ShakeEffectRoutine());
    }

    public IEnumerator RetreatRoutine()
    {
        yield return null;
    }

    private void Update()
    {
        CheckTrainSpeed();
    }

    private void CheckTrainSpeed()
    {
        switch (_trainData.CurrentTrainSpeed)
        {
            case 0:
                SetMovementIntensity(_carData.EnemyToStopTrain.x, _carData.EnemyToStopTrain.y);
                break;
            case < 50:
                SetMovementIntensity(_carData.EnemyToSlowTrainSpeed.x, _carData.EnemyToSlowTrainSpeed.y);
                break;
            case > 50 and < 100:
                SetMovementIntensity(_carData.EnemyToNormalTrainSpeed.x, _carData.EnemyToNormalTrainSpeed.y);
                break;
            case > 100:
                SetMovementIntensity(_carData.EnemyToFastTrainSpeed.x, _carData.EnemyToFastTrainSpeed.y);
                break;
        }
    }

    private void SetMovementIntensity(float newLow, float newHigh)
    {
        _carData.DrivingFrequency.x = newLow;
        _carData.DrivingFrequency.y = newHigh;
        _carData.DrivingMagnitude.x = newLow;
        _carData.DrivingMagnitude.y = newHigh;
        _carData.InfiniteMagnitude.x = newLow;
        _carData.InfiniteMagnitude.y = newHigh;
        _carData.PerlinOffset.x = newLow;
        _carData.PerlinOffset.y = newHigh;
        _carData.PerlinSpeed = newHigh;

        if (newHigh == 0) 
        {
            _carDustVFX.SetFloat("FlipbookAlpha", 0);
            _carDustVFX.SetFloat("SmokeAmount", 0);
            _carTrailVFX.SetFloat("FlipbookAlpha", 0);
            _carTrailVFX.SetFloat("SmokeAmount", 0);
        }
        else
        {
            _carDustVFX.SetFloat("FlipbookAlpha", 1);
            _carDustVFX.SetFloat("SmokeAmount", 12);
            _carTrailVFX.SetFloat("FlipbookAlpha", 1);
            _carTrailVFX.SetFloat("SmokeAmount", 12);
        }
    }

    private void CompletedLevelRoutine(bool _)
    {
        _levelCompleted = true;
    }
}