using FMOD;
using UnityEngine;
using FMODUnity;
//tell compiler we want to use Unity Debug class
using Debug = UnityEngine.Debug;

public class SetFMODValue : SetSliderValue
{
    [SerializeField] private string _parameterName = "Volume";

    public override void SetValue(float value)
    {
        base.SetValue(value);

        float remappedValue = SliderValueToOutputValue(value);
        //set global FMOD parameter
        RESULT result = RuntimeManager.StudioSystem.setParameterByName(_parameterName, remappedValue);
        if (result == RESULT.OK) 
        {
            Debug.Log($"FMOD parameter set fail: {result}");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        RESULT result = RuntimeManager.StudioSystem.getParameterByName(_parameterName, out float value);
        if (result == RESULT.OK)
        {
            float sliderValue = OutputValueToSliderValue(value);
            _slider.SetValueWithoutNotify(sliderValue);
        }
    }
}
