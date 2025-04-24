using GameEvents;
using UnityEngine;

public class GUICoalWarning : GUIWarning
{
    [SerializeField] private ReasourceStationDataSO _reasourceStationData;
    [SerializeField] private string _missingobjectMessage = "Missing<br> <b>Bucket</b>";


    public override void WarningOnInteract(Interactor interactor)
    {
        if (_isDisplayingWarning)
            return;

        if (interactor.ObjectInHand == null || interactor.ObjectInHand.PickupableData != _reasourceStationData.RequireItem)
        {
            warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
            WriteWarningMessage(_missingobjectMessage, true);
            _onTaskRequirementsDisplayed?.Invoke(warningInfo);
            return;
        }

    }

}
