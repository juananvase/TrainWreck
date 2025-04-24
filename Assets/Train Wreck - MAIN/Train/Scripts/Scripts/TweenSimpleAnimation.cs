using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class TweenSimpleAnimation : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Animation Data")] private TweenSettings<Vector3> _animation;

    private void OnEnable()
    {
        _animation.startValue = transform.position;
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
    }

    public void DoAnimation() 
    {
        Tween.Position(transform, _animation);
    }
}
