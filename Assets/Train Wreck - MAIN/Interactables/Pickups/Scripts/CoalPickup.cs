using UnityEngine;

public class CoalPickup : Pickupable
{
    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);

        //TODO coal pickupsound
    }

    public override void Drop()
    {
        base.Drop();

        //TODO coal drop sound
    }
}
