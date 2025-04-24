using UnityEngine;
using UnityEngine.UI;

public class AimLineOpacitySetter : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _minOpacity = 0.2f;
    [SerializeField] private float _maxOpacity = 1f;

    private void Update()
    {
        if (_image == null)
        {
            return;
        }

        float alpha = Mathf.PingPong(Time.time * _speed, _maxOpacity - _minOpacity) + _minOpacity;

        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}