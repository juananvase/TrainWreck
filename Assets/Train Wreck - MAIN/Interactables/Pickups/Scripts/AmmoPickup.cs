using FMODUnity;
using UnityEngine;

public class AmmoPickup : Pickupable
{
    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.AmmoPickupSFX, pickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.DropAmmoSFX, transform.position);
    }
}
