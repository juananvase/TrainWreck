using UnityEngine;

public class TriggerIconFirebox : TriggerIcon
{
    [SerializeField] private Firebox _firebox;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_firebox == null)
            Debug.LogWarning("Firebox component missing");
    }

    protected override void CheckDisplayButton(Collider other)
    {
        if (other.TryGetComponent(out Interactor interactor))
        {
            PickupableContainer bucket = interactor.ObjectInHand as PickupableContainer;

            if (bucket != null)
            {
                if(bucket.ItemInContainer != null && _firebox.IsDoorOpen)
                    _icon.SetActive(true);
            }

        }
    }

}
