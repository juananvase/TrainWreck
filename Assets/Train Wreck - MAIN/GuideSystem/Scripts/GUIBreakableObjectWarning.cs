using UnityEngine;

public class GUIBreakableObjectWarning : GUIWarning
{
    [SerializeField] private BreakableObjectDataSO _breakableObjectData;
    [SerializeField] private string _missingObjectMessage = "Missing<br> <b>Material</b>";

    public override void WarningOnInteract(Interactor interactor, RepairState status)
    {
        if (_isDisplayingWarning)
            return;


        if (status != RepairState.Broken)
            return;


        if (interactor.ObjectInHand == null || interactor.ObjectInHand.PickupableData != _breakableObjectData.RequireItem)
        {
            warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
            WriteWarningMessage(_missingObjectMessage, true);
            _onTaskRequirementsDisplayed?.Invoke(warningInfo);
            return;
        }
    }
}
