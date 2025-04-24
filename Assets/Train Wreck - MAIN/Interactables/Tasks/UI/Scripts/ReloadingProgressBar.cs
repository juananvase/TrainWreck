using EditorTools;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ReloadingProgressBar : BaseProgressBar
{
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private MountedGun _mountedGun;
    [SerializeField] private IntEventAsset _onUpdateAmmoCountUI;

    private void OnEnable()
    {
        _fillBar.fillAmount = (float)_mountedGun.CurrentBulletCount / _mountedGun.BulletData.InitialMaxBulletCount ;
        if (_fillBar.fillAmount >= 1f) { gameObject.SetActive(false); }

        _onUpdateAmmoCountUI.AddListener(Reloading);
    }
    private void OnDisable()
    {
        _onUpdateAmmoCountUI.RemoveListener(Reloading);
    }

    private void Reloading(int bulletCount) 
    {
        float percentage = (float)_mountedGun.CurrentBulletCount / _mountedGun.BulletData.InitialMaxBulletCount;

        _lerpCoroutine = LerpBar(percentage, _lerpDuration, _fillBar);
        StartCoroutine(_lerpCoroutine);
    }


}
