using UnityEngine;

public class TriggerIconBreakableObject : TriggerIcon
{
    [SerializeField] private BreakableObject _breakableObject;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_breakableObject == null)
            Debug.LogWarning("BreakableObject component missing");
    }

    protected override void CheckDisplayButton(Collider other)
    {
        if (other.TryGetComponent(out Interactor interactor))
        {
            if (interactor.ObjectInHand == null)
                return;

            if (_breakableObject.RepairStatus == RepairState.Broken && interactor.ObjectInHand.PickupableData == _breakableObject.BreakableObjectData.RequireItem)
            {
                _icon.SetActive(true);
            }

        }
    }
}
