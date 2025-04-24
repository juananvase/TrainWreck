using System.Collections.Generic;
using FMODUnity;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceStation : MonoBehaviour, IInteractable
{
    //Important data and Game events and SOs
    [field: SerializeField, InlineEditor, FoldoutGroup("Data")] public ReasourceStationDataSO ReasourceStationData { get; private set; }
    [SerializeField, FoldoutGroup("Data")] private PickupablesEventAsset _onUpdateSpawnedResourcesList;

    private List<Pickupable> resourcesSpawned = new(); //A list of the resources that are in game -> used to limit the amount of items in game by implementing object pulling

    //Input
    protected float interactInput;

    private void OnEnable()
    {
        _onUpdateSpawnedResourcesList.AddListener(RemoveResourceFromResourcesSpawnedList);

        ReasourceStationData.OnInteractionInput.AddListener(ReceivingInput);
    }
    private void OnDisable()
    {
        _onUpdateSpawnedResourcesList.RemoveListener(RemoveResourceFromResourcesSpawnedList);

        ReasourceStationData.OnInteractionInput.RemoveListener(ReceivingInput);
    }

    private void ReceivingInput(float value)
    {
        interactInput = value;
    }

    public virtual void Interact(Interactor interactor)
    {
        //Only read whe is pressed and not when is release
        if (interactInput < 0.5f)
            return;

        //TODO is this rigth?
        //Check is player is NOT empty handed
        if (interactor.ObjectInHand != null)
            return;

        interactor.TryPickup(SpawnResource());

        //Play sfx
        PlaySFX(ReasourceStationData.AudioData.UseResourceStationSFX);
    }
    protected Pickupable SpawnResource() 
    {
        Pickupable resource;

        //Instantiate an resource implementing object pooling
        resource = PoolSystem.Instance.Get(ReasourceStationData.Resource, transform.position + new Vector3(0, 1, 0), Quaternion.identity) as Pickupable;

        //Update resourcesSpawned list
        UpdateResourcesSpawnedList(resource);

        return resource;
    }
    private void UpdateResourcesSpawnedList(Pickupable resource)
    {
        //Add spawned resource to the list
        resourcesSpawned.Add(resource);

        //Check objects the amount of objects in game, if there are too many it clean the forst one on the list
        if (resourcesSpawned.Count <= 10)
            return;

        //Checks if the resource is on the floor and not in players hands before it cleans it
        foreach (Pickupable pickable in resourcesSpawned)
        {
            if (pickable.IsBeingHeld == false)
            {
                pickable.Cleanup();
                resourcesSpawned.Remove(pickable);
                break;
            }
        }
    }

    //Attached to and event that remove the resource from the list when it is used on a task
    private void RemoveResourceFromResourcesSpawnedList(Pickupable resource)
    {
        resourcesSpawned.Remove(resource);
    }

    public virtual void PlaySFX(EventReference SFX)
    {
        RuntimeManager.PlayOneShot(SFX, transform.position);
    }
}