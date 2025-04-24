using FMODExtensions;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class PickupableContainer : Pickupable, IRespawnable
{
    //Set SO to PickupableContainerDataSO
    public PickupableContainerDataSO PickupableContainerData => PickupableData as PickupableContainerDataSO;

    //Important data and Game events and SOs
    [field: SerializeField, FoldoutGroup("Data")] public Transform PickupAnchor { get; private set; }
    [field: SerializeField, FoldoutGroup("Data")] public Pickupable ItemInContainer { get; private set; } = null;

    //VFX variables
    [SerializeField, FoldoutGroup("VFX")] private Transform _vfxSpawnPoint;

    private GameObject _vfxInstance; //Store only one vfs at the time

    public void LoadContainer(Pickupable storeItem)
    {
        if (ItemInContainer != null)
            return;

        ItemInContainer = storeItem;

        GetComponent<PickupableContainerAudioHandler>()._isFull = true;

        ItemInContainer.Collider.enabled = false;
        ItemInContainer.Rigidbody.isKinematic = true;

        ItemInContainer.transform.SetParent(transform);
        ItemInContainer.transform.SetPositionAndRotation(PickupAnchor.position, PickupAnchor.rotation);

        PlayVFX(_vfxSpawnPoint, PickupableContainerData.VFX);
    }
    public void EmptyContainer()
    {
        ItemInContainer.Cleanup();
        ItemInContainer = null;

        CleanUpVFX(_vfxInstance);

        GetComponent<PickupableContainerAudioHandler>()._isFull = false;
    }

    private void PlayVFX(Transform spawnPoint, GameObject vfx)
    {
        if (vfx == null)
        {
            Debug.LogError("Missing VFX prefab");
            return;
        }

        if (_vfxSpawnPoint == null)
        {
            Debug.LogError("Missing vfxSpawnPoitn transform");
            return;
        }

        _vfxInstance = Instantiate(vfx, _vfxSpawnPoint.position, _vfxSpawnPoint.rotation);
        _vfxInstance.transform.SetParent(ItemInContainer.Collider.transform);
    }
    private void CleanUpVFX(GameObject vfxInstance)
    {
        if (_vfxInstance != null)
        {
            Destroy(_vfxInstance);
            _vfxInstance.transform.SetParent(null);
        }
    }

    public override void Pickup(Transform pickupAnchor)
    {
        base.Pickup(pickupAnchor);
        RuntimeManager.PlayOneShot(AudioData.PickupBucketSFX, PickupAnchor.position);
    }

    public override void Drop()
    {
        base.Drop();
        RuntimeManager.PlayOneShot(AudioData.DropBucketSFX, PickupAnchor.position);
    }
}