using System.Collections;
using EditorTools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class OpenCloseUI : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Data")] private bool _isOpen;
    [SerializeField, FoldoutGroup("Data"), AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private RectTransform _canvasRectTrannsform;
    [SerializeField, FoldoutGroup("Data"), AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private CanvasGroup _canvasGroup;

    [SerializeField, FoldoutGroup("Animation Data")] private bool _onlyTargetX = false;
    [SerializeField, FoldoutGroup("Animation Data")] private bool _onlyTargetY = false;
    [SerializeField, FoldoutGroup("Animation Data")] private bool _onCloseTargetAlphaCanvasGroup = false;
    [SerializeField, FoldoutGroup("Animation Data")] private bool _onOpenTargetAlphaCanvasGroup = false;
    [SerializeField, FoldoutGroup("Animation Data")] private TweenSettings<Vector2> _OpenAnimation;
    [SerializeField, FoldoutGroup("Animation Data")] private TweenSettings<Vector2> _CloseAnimation;

    private void OnEnable()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
    }
    private void UpdateIgnoredAxis()
    {
        if (!_onlyTargetX && !_onlyTargetY) return;

        if (_onlyTargetX)
        {
            _OpenAnimation.startValue.y = _canvasRectTrannsform.anchoredPosition.y;
            _OpenAnimation.endValue.y = _canvasRectTrannsform.anchoredPosition.y;

            _CloseAnimation.startValue.y = _canvasRectTrannsform.anchoredPosition.y;
            _CloseAnimation.endValue.y = _canvasRectTrannsform.anchoredPosition.y;
        }
        else if (_onlyTargetY)
        {
            _OpenAnimation.startValue.x = _canvasRectTrannsform.anchoredPosition.x;
            _OpenAnimation.endValue.x = _canvasRectTrannsform.anchoredPosition.x;

            _CloseAnimation.startValue.x = _canvasRectTrannsform.anchoredPosition.x;
            _CloseAnimation.endValue.x = _canvasRectTrannsform.anchoredPosition.x;
        }
    }


    [PropertySpace(25), Button(ButtonSizes.Large, Icon = SdfIconType.Toggles), GUIColor(0.5f, 0.5f, 0.9f)]
    public void ToggleOpenCLose()
    {
        if (_isOpen)
            CloseCanvas();
        else
            OpenCanvas();
    }

    [Button(Icon = SdfIconType.ToggleOn)]
    public void OpenCanvas()
    {
        _isOpen = true;

        gameObject.SetActive(true);

        if (_canvasGroup != null)
        {
            if (_onOpenTargetAlphaCanvasGroup)
            {
                Sequence.Create(cycles: 1, useUnscaledTime: true)
                .Group(Tween.UIAnchoredPosition(_canvasRectTrannsform, _CloseAnimation))
                .Group(Tween.Custom(0f, 1f, duration: _CloseAnimation.settings.duration, value => _canvasGroup.alpha = value)).OnComplete(() => gameObject.SetActive(true), warnIfTargetDestroyed: false);
            }

            _canvasGroup.alpha = 1f;
            Tween.UIAnchoredPosition(_canvasRectTrannsform, _CloseAnimation).OnComplete(() => gameObject.SetActive(true), warnIfTargetDestroyed: false);
            return;
        }

        Tween.UIAnchoredPosition(_canvasRectTrannsform, _CloseAnimation).OnComplete(() => gameObject.SetActive(true), warnIfTargetDestroyed: false);

        UpdateIgnoredAxis();
    }

    [Button(Icon = SdfIconType.ToggleOff)]
    public void CloseCanvas()
    {
        _isOpen = false;

        UpdateIgnoredAxis();

        if (_canvasGroup != null)
        {
            if (_onCloseTargetAlphaCanvasGroup)
            {
                Sequence.Create(cycles: 1, useUnscaledTime: true)
                .Group(Tween.UIAnchoredPosition(_canvasRectTrannsform, _OpenAnimation))
                .Group(Tween.Custom(1f, 0f, duration: _OpenAnimation.settings.duration, value => _canvasGroup.alpha = value)).OnComplete(() => gameObject.SetActive(false), warnIfTargetDestroyed: false);
            }

            Tween.UIAnchoredPosition(_canvasRectTrannsform, _OpenAnimation).OnComplete(() => gameObject.SetActive(false), warnIfTargetDestroyed: false);

            return;
        }

        Tween.UIAnchoredPosition(_canvasRectTrannsform, _OpenAnimation).OnComplete(() => gameObject.SetActive(false), warnIfTargetDestroyed: false);

    }
}