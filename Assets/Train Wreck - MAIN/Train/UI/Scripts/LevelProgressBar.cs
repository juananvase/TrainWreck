using System.Collections;
using GameEvents;
using Remaping;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] private TrainDataSO _trainData;
    [SerializeField] private FireboxTaskDataSO _fireboxData;
    [SerializeField] private Slider _slider;
    [SerializeField] private BoolEventAsset _OnLevelWin;

    private float _speed = 0f;
    private float _speedMultiplier = 0f;

    [FoldoutGroup("Events")] public UnityEvent OnFinishRoute;

    private void OnEnable()
    {
        if (_fireboxData.OnFuelLevelUpdated == null) 
        { 
            Debug.Log("No GameEvent Assign"); 
            return; 
        }

        //_fireboxData.OnFuelLevelUpdated.OnInvoked.AddListener(CheckGaugeState);

    }
    private void OnDisable()
    {
        if (_fireboxData.OnFuelLevelUpdated == null) 
        {
            Debug.Log("No GameEvent Assign"); 
            return; 
        }

        //_fireboxData.OnFuelLevelUpdated.OnInvoked.RemoveListener(CheckGaugeState);
    }

    private void Start()
    {
        StartCoroutine(UpdateSliderValue());
    }

    //private void CheckGaugeState(float fuelLevel)
    //{
    //    if (fuelLevel < _fireboxData.LowThreshold)
    //    {
    //        _speedMultiplier = 0;
    //        return;
    //    }

    //    if (fuelLevel >= _fireboxData.LowThreshold && fuelLevel < _fireboxData.MediumThreshold)
    //    {

    //        _speedMultiplier = _trainData.LowStateSpeed;
    //        return;
    //    }

    //    if (fuelLevel >= _fireboxData.MediumThreshold && fuelLevel < _fireboxData.OptimumThreshold)
    //    {

    //        _speedMultiplier = _trainData.MediumStateSpeed;
    //        return;
    //    }

    //    if (fuelLevel >= _fireboxData.OptimumThreshold && fuelLevel < _fireboxData.OverclockThreshold)
    //    {

    //        _speedMultiplier = _trainData.OptimumStateSpeed;
    //        return;
    //    }

    //    if (fuelLevel >= _fireboxData.OverclockThreshold)
    //    {

    //        _speedMultiplier = _trainData.OverclockStateSpeed;
    //        return;
    //    }
    //}

    private IEnumerator UpdateSliderValue()
    {
        while (true) 
        {
            _speedMultiplier = Remap.FloatRemap(_fireboxData.LowThreshold, _fireboxData.OverloadThreshold, 0, _trainData.OverclockStateSpeed, _fireboxData.CurrentFuelLevel);
            _speed += Time.deltaTime * _speedMultiplier;
            float progress = _speed / _trainData.DistanceToTravel;
            _slider.value = progress;

            //int distanceTraveled = (int)(progress * 100);
            //_progressIndicator.SetText(distanceTraveled + " Km");

            if (_slider.value == 1)
            {
                yield return null;
                _slider.value = 0;
                _OnLevelWin?.Invoke(true);
                OnFinishRoute?.Invoke();
            }

            yield return null;
        }
    }
}
