using UnityEngine;

public interface IPickupable : IInteractable
{
    void Pickup(Transform pickUpAnchorPoint);
    void Drop();
}