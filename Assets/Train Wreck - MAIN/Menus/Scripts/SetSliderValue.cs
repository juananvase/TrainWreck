using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderValue : SerializedMonoBehaviour
{
    [SerializeField] protected Vector2 _outMinMax = new Vector2(-0.5f, 0.5f);
    [SerializeField] protected Slider _slider;

    private void OnValidate()
    {
        _slider = GetComponent<Slider>();
    }

    protected virtual void OnEnable()
    {
        _slider.onValueChanged.AddListener(SetValue);
    }

    protected virtual void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(SetValue);
    }

    public virtual void SetValue(float value) { }

    protected float SliderValueToOutputValue(float value)
    {
        //remap from slider to outMinMax range
        float Percentage = value /_slider.maxValue;
        float remappedValue = Mathf.Lerp(_outMinMax.x, _outMinMax.y, Percentage);
        return remappedValue;
    }

    protected float OutputValueToSliderValue(float value)
    {
        //remap from outMinMax to slider range
        float percentage = Mathf.InverseLerp(_outMinMax.x, _outMinMax.y, value);
        float sliderValue = percentage * _slider.maxValue;
        return sliderValue;
    }

}
