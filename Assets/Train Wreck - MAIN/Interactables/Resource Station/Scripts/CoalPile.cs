using EditorTools;
using UnityEngine;

public class CoalPile : ResourceStation
{
    [SerializeField, AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] private GUICoalWarning _warning;

    public override void Interact(Interactor interactor)
    {
        //Only read whe is pressed and not when is release
        if (interactInput < 0.5f)
            return;

        _warning.WarningOnInteract(interactor);

        //Check if we are interacting with a container
        if (interactor.ObjectInHand is PickupableContainer container)
        {
            //TODO is this rigth?
            //Check is container is empty
            if (container.ItemInContainer != null)
                return;

            //Check if player have the require item to use this RS
            if (interactor.ObjectInHand.PickupableData != ReasourceStationData.RequireItem)
                return;

            container.LoadContainer(SpawnResource());

            //Play sfx
            PlaySFX(ReasourceStationData.AudioData.UseCoalPileSFX);
        }
    }
}
