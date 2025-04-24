using System.Collections;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Remaping;
using EditorTools;

public class HeallthBar : BaseProgressBar
{
    [SerializeField] private Vector2EventAsset _onTrainHealthUpdated;

    [SerializeField] private GameObject _targetShake;
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private ShakeUI _shake;


    //Colors
    [SerializeField, FoldoutGroup("Colors")] private Color _onLowHealthColor;
    [SerializeField, FoldoutGroup("Colors")] private Color _onHighHealthColor;

    private float _healthPercentage = 1;

    private void OnEnable()
    {
        _fillBar.fillAmount = 1;
        _onTrainHealthUpdated.AddListener(OnHealthUpdated);
    }

    private void OnDisable()
    {
        _onTrainHealthUpdated.RemoveListener(OnHealthUpdated);
    }

    private void Start()
    {
        StartCoroutine(FeedbackCoroutine());
    }
    private IEnumerator FeedbackCoroutine()
    {
        while (true)
        {
            _fillBar.color = Remap.ColorRemap(0.3f, 0.5f, _onLowHealthColor, _onHighHealthColor, _healthPercentage);

            yield return null;
        }
    }

    private void Damaged(float percentage)
    {
        if (_lerpCoroutine != null) StopCoroutine(_lerpCoroutine);
        _lerpCoroutine = LerpBar(percentage, _lerpDuration, _fillBar);
        StartCoroutine(_lerpCoroutine);
        _shake.ShakeObject(_targetShake);
    }

    private void Healed(float percentage)
    {
        if (_lerpCoroutine != null) StopCoroutine(_lerpCoroutine);
        _lerpCoroutine = LerpBar(percentage, _lerpDuration, _fillBar);
        StartCoroutine(_lerpCoroutine);
    }

    private void OnHealthUpdated(Vector2 arg0)
    {
        _healthPercentage = arg0.x / arg0.y;
        if (_healthPercentage > _fillBar.fillAmount)
        {
            Healed(_healthPercentage);
        }
        else
        {
            Damaged(_healthPercentage);
        }
    }
}
