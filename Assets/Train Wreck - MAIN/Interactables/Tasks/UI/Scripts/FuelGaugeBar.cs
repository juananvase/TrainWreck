using Remaping;
using UnityEngine;
using UnityEngine.UI;

public class FuelGaugeBar : MonoBehaviour
{
    [SerializeField] private FireboxTaskDataSO _fireboxData;
    [SerializeField] private Image _fill;
    [SerializeField] private Color _lowHeatColor;
    [SerializeField] private Color _mediumHeatColor;
    [SerializeField] private Color _highHeatColor;


    private void OnEnable()
    {
        if (_fireboxData.OnFuelLevelUpdated == null) { Debug.Log("No GameEvent Assign"); return; }
        _fireboxData.OnFuelLevelUpdated.OnInvoked.AddListener(UpdateGaugeLevel);
    }
    private void OnDisable()
    {
        if (_fireboxData.OnFuelLevelUpdated == null) { Debug.Log("No GameEvent Assign"); return; }
        _fireboxData.OnFuelLevelUpdated.OnInvoked.RemoveListener(UpdateGaugeLevel);
    }

    private void UpdateGaugeLevel(float fuelLevel) 
    {
        float fuelLevelPercentage = _fireboxData.CurrentFuelLevel/_fireboxData.MaxFuelLevel;
        _fill.fillAmount = fuelLevelPercentage;

        if (_fireboxData.CurrentFuelLevel < _fireboxData.MediumThreshold)
        {
            _fill.color = Remap.ColorRemap(_fireboxData.LowThreshold, _fireboxData.MediumThreshold, _lowHeatColor, _mediumHeatColor, _fireboxData.CurrentFuelLevel);
        }
        else 
        {
            _fill.color = Remap.ColorRemap(_fireboxData.MediumThreshold, _fireboxData.OverloadThreshold, _mediumHeatColor, _highHeatColor, _fireboxData.CurrentFuelLevel); 
        }

    }
}