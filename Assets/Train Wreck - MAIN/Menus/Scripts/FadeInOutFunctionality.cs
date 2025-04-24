using System.Collections;
using GameEvents;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeInOutFunctionality : MonoBehaviour
{
    [SerializeField] private RectTransform _circle;
    [SerializeField] private bool _startOnFadeIn = true;
    [SerializeField, FoldoutGroup("Animation Data")] private TweenSettings<Vector2> _fadeInAnimation;
    [SerializeField, FoldoutGroup("Animation Data")] private TweenSettings<Vector2> _fadeOutAnimation;

    public UnityEvent _onFinishFadeIn;
    public UnityEvent _onFinishFadeOut;

    private void OnEnable()
    {
        if (_startOnFadeIn)
        {
            FadeIn();
            return;
        }

        FadeOut();
    }

    public void FadeIn()
    {
        Tween.UISizeDelta(_circle, _fadeInAnimation).OnComplete(() => _onFinishFadeIn.Invoke(), warnIfTargetDestroyed: false);
    }

    public void FadeOut()
    {
        Tween.UISizeDelta(_circle, _fadeOutAnimation).OnComplete(() => _onFinishFadeOut.Invoke(), warnIfTargetDestroyed: false);

    }
}
