using GameEvents;
using UnityEngine;

public class GUIFireboxWarning : GUIWarning
{
    [SerializeField] private FireboxTaskDataSO _fireBoxData;

    [SerializeField] private BoolEventAsset _onLeverSwitch;
    [SerializeField] private BoolEventAsset _onCleanWidget;

    [SerializeField] protected WarningType _doorWarningType;
    [SerializeField] protected WarningType _emptyFuelWarningType;
    [SerializeField] protected WarningType _explosionlWarningType;
    private WarningInfo _doorWarningInfo => new(_doorWarningType, Color.white);
    private WarningInfo _emptyFuelWarningInfo => new(_emptyFuelWarningType, Color.white);
    private WarningInfo _explosionWarningInfo => new(_explosionlWarningType, Color.white);

    [SerializeField] private string _missingobjectMessage = "Missing<br> <b>Bucket</b> of <b>Coal</b>";
    [SerializeField] private string _closeDoorMessage = "<b>Door</b><br> is Close";
    
    private bool _isDoorOpen= false;

    private void OnEnable()
    {
        if (_onLeverSwitch == null) { Debug.Log("No GameEvent Assign"); return; }
        _onLeverSwitch.OnInvoked.AddListener(SetDoorState);

    }
    private void OnDisable()
    {
        if (_onLeverSwitch == null) { Debug.Log("No GameEvent Assign"); return; }
        _onLeverSwitch.OnInvoked.RemoveListener(SetDoorState);

    }

    private void SetDoorState(bool isOpen)
    {
        _isDoorOpen = isOpen;
    }

    public override void WarningOnInteract(Interactor interactor)
    {
        if (_isDisplayingWarning)
            return;

        if (interactor.ObjectInHand == null) 
        {
            WriteWarningMessage(_missingobjectMessage, true);

            warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
            _onTaskRequirementsDisplayed?.Invoke(warningInfo);
            return;
        }

        switch (interactor.ObjectInHand.TryGetComponent(out PickupableContainer container))
        {
            case true:

                if (container.ItemInContainer == null || container.ItemInContainer.PickupableData != _fireBoxData.RequireItem)
                {
                    warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
                    WriteWarningMessage(_missingobjectMessage, true);
                    _onTaskRequirementsDisplayed?.Invoke(warningInfo);
                    return;
                }

                break;

            case false:

                warningInfo = new WarningInfo(_warningType, interactor.CharacterColor);
                WriteWarningMessage(_missingobjectMessage, true);
                _onTaskRequirementsDisplayed?.Invoke(warningInfo);
                return;
        }

        if (_isDoorOpen == false) 
        {
            WriteWarningMessage(_closeDoorMessage, true);
            _onTaskRequirementsDisplayed?.Invoke(_doorWarningInfo);
            return;
        }
    }

    public void WarningOnEmptyFuel(bool isEmpty) 
    {
        if (isEmpty)
        {
            _onTaskRequirementsDisplayed?.Invoke(_emptyFuelWarningInfo);
            return;
        }

        _onCleanWidget.Invoke(isEmpty);
    }

    public void WarningOnExplosion()
    {
        _onTaskRequirementsDisplayed?.Invoke(_explosionWarningInfo);
    }
}
