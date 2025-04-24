using System;
using System.Collections;
using GameEvents;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

public class OnTaskRequirementsDisplayListener : MonoBehaviour
{
    [SerializeField] private WarningInfoEventAsset _onTaskRequirementsDisplayed;
    [SerializeField] private BoolEventAsset _onCleanWidget;
    [SerializeField] private WarningType[] _warnings;
    [SerializeField] private GameObject _icon;
    [SerializeField] private SpriteRenderer _ring;
    [SerializeField] private bool _displayRing = true;
    [SerializeField] private TweenSettings<Vector3> _ringTweenSetting;
    [SerializeField] private float _widgetCleaningTime = 2;

    private void OnEnable()
    {
        _onTaskRequirementsDisplayed.AddListener(DisplayFeedback);
        _onCleanWidget.AddListener(CleanWidget);
    }
    private void OnDisable()
    {
        _onTaskRequirementsDisplayed.RemoveListener(DisplayFeedback);
        _onCleanWidget.RemoveListener(CleanWidget);
    }

    private void DisplayFeedback(WarningInfo warningInfo)
    {
        foreach (WarningType warningType in _warnings) 
        {
            if (warningInfo.WarningType != warningType) 
            {
                continue;
            }

            _icon.gameObject.SetActive(true);
            StartCoroutine(CleanWidgetAfterSeconds(_widgetCleaningTime, _onCleanWidget));
            RingIndicator(_displayRing, warningInfo.Color);
            break;
        }

    }

    private void RingIndicator(bool displayRing, Color ringColor) 
    {
        if (displayRing == false)
            return;

        _ring.color = ringColor;
        _ring.gameObject.SetActive(true);
        Tween.Scale(_ring.transform, _ringTweenSetting).OnComplete(() => _ring.gameObject.SetActive(false));
    }

    private IEnumerator CleanWidgetAfterSeconds(float seconds, bool cleanWidget)
    {
        yield return new WaitForSeconds(seconds);

        if(!cleanWidget) _icon.gameObject.SetActive(false);
    }

    public void CleanWidget(bool value) 
    {
        _icon.gameObject.SetActive(value);
    }
}
