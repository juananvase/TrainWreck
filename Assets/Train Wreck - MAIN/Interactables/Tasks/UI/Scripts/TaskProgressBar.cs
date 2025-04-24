using System.Collections;
using EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class TaskProgressBar : BaseProgressBar
{
    //AutoAssign automatically finds components when add this script
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private Health _health;

    [SerializeField] private GameObject _targetShake;
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private ShakeUI _shake;


    private void OnEnable()
    {
        _fillBar.fillAmount = _health.Percentage;
        if (_fillBar.fillAmount >= 1f) { gameObject.SetActive(false); }

        _health?.OnDamage.AddListener(Damaged);
        _health?.OnHealing.AddListener(Fixed);
    }


    private void OnDisable()
    {
        _health?.OnDamage.RemoveListener(Damaged);
        _health?.OnHealing.RemoveListener(Fixed);
    }

    private void Damaged(DamageInfo damageInfo)
    {
        if (!gameObject.activeSelf) { return; }
        if (_lerpCoroutine != null) StopCoroutine(_lerpCoroutine);
        _lerpCoroutine = LerpBar(damageInfo.FinalPercentage, _lerpDuration, _fillBar);
        StartCoroutine(_lerpCoroutine);
    }

    private void Fixed(HealingInfo healingInfo)
    {
        _shake.ShakeObject(_targetShake);

        if (!gameObject.activeSelf) { return; }
        if (_lerpCoroutine != null) StopCoroutine(_lerpCoroutine);
        _lerpCoroutine = LerpBar(healingInfo.FinalPercentage, _lerpDuration, _fillBar);
        StartCoroutine(_lerpCoroutine);
    }
}
