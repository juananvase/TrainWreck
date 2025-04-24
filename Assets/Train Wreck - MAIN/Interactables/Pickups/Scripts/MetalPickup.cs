using FMODUnity;
using UnityEngine;
public class MetalPickup : Pickupable
{
    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.PickupPipeSFX, pickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.PickupPipeSFX, transform.position);
    }
}
