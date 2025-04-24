using System;
using System.Collections;
using FMODUnity;
using GameEvents;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class Lever : Task
{
    [SerializeField] private GameObject _lever;
    [SerializeField] private float _timeToInactivated;
    [field: SerializeField] public BoolEventAsset OnLeverSwitch { get; private set; }
    private bool _isLeverActive = false;

    [field: SerializeField] public BoolEventAsset OnTriggerLever { get; private set; }
    private Coroutine _triggerLevelCoroutine = null;

    protected override void OnEnable()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        OnTriggerLever.AddListener(TriggerLever);
    }

    protected override void OnDisable()
    {
        OnTriggerLever.RemoveListener(TriggerLever);
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        if (_isLeverActive || interactor.ObjectInHand != null) return;

        TriggerLever(true);
    }

    [Button(Icon = SdfIconType.ToggleOn)]
    private void TriggerLever(bool value) 
    {
        if (_triggerLevelCoroutine != null)
        {
            StopCoroutine(_triggerLevelCoroutine);
        }

        PlaySFX(TaskData.AudioData.ActiveLeverSFX);
        _triggerLevelCoroutine = StartCoroutine(WaitForSecondsToInactiveLever(_timeToInactivated));
    }

    private void ActiveLever()
    {
        _isLeverActive = true;
        OnLeverSwitch?.Invoke(_isLeverActive);
    }

    private void InactiveLever()
    {
        _triggerLevelCoroutine = null;
        
        _isLeverActive = false;
        OnLeverSwitch?.Invoke(_isLeverActive);
        PlaySFX(TaskData.AudioData.InActiveLeverSFX);
    }

    private IEnumerator WaitForSecondsToInactiveLever(float seconds)
    {
        Tween.LocalRotation(_lever.transform, endValue: new Vector3(0, 0, 65), duration: 0.5f).OnComplete(() => ActiveLever(), warnIfTargetDestroyed: false);

        ///TODO add clock tick SFX
        yield return Tween.LocalRotation(_lever.transform, endValue: new Vector3(0, 0, -65), duration: seconds, startDelay: 0.5f).OnComplete(() => InactiveLever(), warnIfTargetDestroyed: false).ToYieldInstruction();
    }
}
