using FMODUnity;
using UnityEngine;

public class WoodPickup : Pickupable
{
    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.PickupWoodSFX, pickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.DropWoodSFX, transform.position);
    }
}
