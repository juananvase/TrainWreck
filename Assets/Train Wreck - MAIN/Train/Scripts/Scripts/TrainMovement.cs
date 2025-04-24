using System.Collections;
using FMOD.Studio;
using UnityEngine;
using GameEvents;
using CustomFMODFunctions;
using FMODUnity;
using Remaping;

public class TrainMovement : TrainBase
{
    [Header("Train Animation Settings")]
    [SerializeField] private float _amplitude = 1.0f;
    [SerializeField] private float _offset;
    [Header("Train Animation Frequency")]
    [SerializeField] private float _whenSlowFreq;
    [SerializeField] private float _whenNormalFreq;
    [SerializeField] private float _whenFastFreq;
    [Header("Data")]
    [SerializeField] private AudioSourcesSOData _audioData;
    [SerializeField] private FireboxTaskDataSO _fireboxData;
    [Header("Events")]
    [SerializeField] private FloatEventAsset OnFuelLevelUpdated;
    [SerializeField] private FloatEventAsset OnTrainSpeedUpdated;

    
    private Vector3 _startPosition;
    private float _currentFrequency = 1f;
    private float _wavePhase; // tracks the wave to have smooth transitions between frequency
    private TrainStages _trainSpeedStages;
    private EventInstance _trainMovingSFXinstance;
    private EventInstance _trainIdleSFXinstance;
    private Coroutine _trainSpeedCoroutine;
    private bool _isPlayingChooChoo;

    private void Start()
    {
        _startPosition = transform.position;
        trainData.CurrentTrainSpeed = 0;
        _trainIdleSFXinstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_trainIdleSFXinstance, _audioData.TrainIdleSFX);
        _trainMovingSFXinstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_trainMovingSFXinstance, _audioData.TrainMovingSFX);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnFuelLevelUpdated.AddListener(UpdateSpeed);
    }
    
    
    protected override void OnDisable()
    {
        base.OnDisable();
        OnFuelLevelUpdated.RemoveListener(UpdateSpeed);

        if (_trainSpeedCoroutine != null)
            StopCoroutine(_trainSpeedCoroutine);

        if (trainData.CurrentTrainSpeed > 0)
            trainData.CurrentTrainSpeed = 0;
        
        AudioInstanceHandler.StopAndReleaseSFXInstance(_trainMovingSFXinstance);
        AudioInstanceHandler.StopAndReleaseSFXInstance(_trainIdleSFXinstance);
    }

    private void Update()
    {
        CheckTrainStages();
        _wavePhase += (Time.deltaTime * (_currentFrequency * Mathf.PI * 2)); // converts freq to radians, completes one full oscillation
        // sin wave movement 
        float newY = _startPosition.y + Mathf.Sin(_wavePhase) * _amplitude + _offset;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
        
        // set train moving sfx speed based on train speed
        RuntimeManager.StudioSystem.setParameterByName("TrainSpeed", trainData.CurrentTrainSpeed);
    }
    private void UpdateSpeed(float fuelLevel)
    {
        if (_trainSpeedCoroutine != null)
            StopCoroutine(_trainSpeedCoroutine);
        
        _trainSpeedCoroutine = StartCoroutine(SetTrainSpeed(fuelLevel));
    }
    
    private IEnumerator SetTrainSpeed(float targetSpeed)
    {
        float currentSpeed = 0f;
        while (!Mathf.Approximately(currentSpeed, targetSpeed))
        {
            currentSpeed = Remap.FloatRemap(_fireboxData.LowThreshold, _fireboxData.OverloadThreshold, 0f, trainData.TrainMaxSpeed,
                _fireboxData.CurrentFuelLevel);
            trainData.CurrentTrainSpeed = currentSpeed;
            OnTrainSpeedUpdated.Invoke(currentSpeed);
            yield return null;
        }
    }   

    private void CheckTrainStages()
    {
        if(trainData == null)
            return;

        switch (trainData.CurrentTrainSpeed)
        {
            case 0:
                SetMovementAnimFrequency(0);
                break;
            case < 50:
                SetMovementAnimFrequency(_whenSlowFreq);
                break;
            case > 50 and < 100:
                SetMovementAnimFrequency(_whenNormalFreq);
                break;
            case > 100:
                SetMovementAnimFrequency(_whenFastFreq);
                if (!_isPlayingChooChoo)
                {
                    StartCoroutine(PlayChooChooSFX());
                }
                break;
        }
    }
    
    private void SetMovementAnimFrequency(float value) => _currentFrequency = value;

    private IEnumerator PlayChooChooSFX()
    {
        _isPlayingChooChoo = true;
        float duration = Random.Range(5, 6);
        RuntimeManager.PlayOneShot(_audioData.ChooChooSFX, transform.position);
        yield return new WaitForSeconds(duration);
        _isPlayingChooChoo = false;
    }
}
