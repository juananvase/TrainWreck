using System;
using GameEvents;
using TMPro;
using UnityEngine;


public class GUIMachineGunWarning : GUIWarning
{
    [SerializeField] private TextMeshProUGUI _bulletCounterMessage;

    [SerializeField] MountedGunDataSO MountedGunData;
    [SerializeField] BulletDataSO BulletData;
    [SerializeField] private IntEventAsset _onUpdateAmmoCountUI;

    [SerializeField] private string _missingobjectMessage = "Missing<br> <b>Ammo</b>";
    

    public void UpdateAmmunitionCountMessage(int ammoCount)
    {
        _bulletCounterMessage.SetText(ammoCount+"/"+BulletData.InitialMaxBulletCount);
    }

    public override void WarningOnInteract(Interactor interactor, bool fullyReloaded, int bulletCount)
    {
        if (_isDisplayingWarning)
            return;

        if (interactor.ObjectInHand == null && bulletCount == 0) 
        {
            warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
            WriteWarningMessage(_missingobjectMessage, true);
            _onTaskRequirementsDisplayed?.Invoke(warningInfo);
            return;
        }

        if (interactor.ObjectInHand != null && fullyReloaded == false)
        {
            if (interactor.ObjectInHand.PickupableData != MountedGunData.RequireItem)
            {
                warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
                WriteWarningMessage(_missingobjectMessage, true);
                _onTaskRequirementsDisplayed?.Invoke(warningInfo);
                return;
            }
        }
    }
}
