using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SetGammaValue : SetSliderValue
{
    [SerializeField] private VolumeProfile _profile;

    public override void SetValue(float value)
    {
        base.SetValue(value);

        //check for existing LiftGammaGain component
        if (_profile.TryGet(out LiftGammaGain liftGammaGain))
        {
            float gammaValue = SliderValueToOutputValue(value);
            liftGammaGain.gamma.value = Vector4.one * gammaValue;
        }
        else 
        {
            Debug.LogWarning("No LiftGammaGain component found on volume!", _profile);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //check for correct profile, then set slider value
        if (_profile.TryGet(out LiftGammaGain liftGammaGain)) 
        {
            float currentGamma = liftGammaGain.gamma.value.x;
            float sliderValue = OutputValueToSliderValue(currentGamma);

            //set slider value (without triggering UnityEvents)
            _slider.SetValueWithoutNotify(sliderValue);
        }
    }
}
