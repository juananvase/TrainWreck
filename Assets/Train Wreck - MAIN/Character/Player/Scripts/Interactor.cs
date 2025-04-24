using GameEvents;
using Sirenix.OdinInspector;
using FMODUnity;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    //Important data and Game events and SOs
    [field: SerializeField, FoldoutGroup("Data"), InlineEditor] public CharacterDataSO CharacterData { get; private set; } = null;
    [SerializeField, FoldoutGroup("Data")] private BoolEventAsset OnSetPlayerFixing; //Set player to fixing from BreakableObject
    [SerializeField, FoldoutGroup("Data")] private BoolEventAsset OnPlayerCaughtOnFire; //Set player to fixing from BreakableObject
    [field: SerializeField, FoldoutGroup("Data")] public Color CharacterColor { get; private set; }

    [SerializeField] private AudioSourcesSOData AudioData;

    [field: SerializeField, FoldoutGroup("PickupDrop")] public Collider PlayerPickupCollider { get; private set; }
    [field: SerializeField, FoldoutGroup("PickupDrop")] public Transform PickupAnchor { get; private set; }
    [field: SerializeField, FoldoutGroup("PickupDrop")] public Pickupable ObjectInHand { get; private set; } = null;
    
    public bool IsFixing { get; private set; } = false;
    private bool _isPlayerOnFire  = false;

    private void OnEnable()
    {
        OnSetPlayerFixing.AddListener(SetPlayerFixing);
        OnPlayerCaughtOnFire.AddListener(SetPlayerCaughtOnFire);
        CharacterData.OnBombExplodeInHand.AddListener(EmptyPlayersHands);
    }
    private void OnDisable()
    {
        OnSetPlayerFixing.RemoveListener(SetPlayerFixing);
        OnPlayerCaughtOnFire.RemoveListener(SetPlayerCaughtOnFire);
        CharacterData.OnBombExplodeInHand.RemoveListener(EmptyPlayersHands);

    }

    private void EmptyPlayersHands(GameObject anchor)
    {
        if (anchor.transform == PickupAnchor) 
        {
            ObjectInHand = null;
            PlayerPickupCollider.enabled = false;
        }
    }

    private void SetPlayerFixing(bool value)
    {
        IsFixing = value;
    }
    private void SetPlayerCaughtOnFire(bool value)
    {
        _isPlayerOnFire = value;    
    }

    public void HandleInteraction()
    {
        Interactor otherPlayer = FindClosestInteractor();

        if (TrySteal(otherPlayer))
            return;

        GameObject targetObject = FindClosestInteractable();

        if (targetObject == null)
            return;

        IInteractable interactableObject = targetObject.GetComponent<IInteractable>();
        Pickupable pickupableObject = interactableObject as Pickupable;

        if (pickupableObject != null && !_isPlayerOnFire)
        {
            TryPickup(pickupableObject);
        }
        if (_isPlayerOnFire && pickupableObject is Extinguisher)
        {
            TryPickup(pickupableObject);
        }
        else if(!_isPlayerOnFire)
        {
            interactableObject.Interact(this);
        }
    }

    private bool TrySteal(Interactor otherPlayer)
    {
        if (otherPlayer == null || ObjectInHand != null || otherPlayer.ObjectInHand == null)
            return false;

        IPickupable pickupInterface = otherPlayer.ObjectInHand;

        pickupInterface.Interact(this);
        pickupInterface.Pickup(PickupAnchor);

        PlayerPickupCollider.enabled = true;
        ObjectInHand = otherPlayer.ObjectInHand;

        otherPlayer.PlayerPickupCollider.enabled = false;
        otherPlayer.ObjectInHand = null;

        return true;
    }

    public void TryUsingItem(bool inputPressed)
    {
        if (ObjectInHand != null)
        {
            if (ObjectInHand.TryGetComponent(out IActivatable activatable))
            {
                activatable.Activate(inputPressed);
            }
        }
    }

    public void TryPickup(Pickupable pickupObject)
    {
        if (ObjectInHand != null)
            return;

        IPickupable pickupInterface = pickupObject;

        pickupInterface.Interact(this);
        pickupInterface.Pickup(PickupAnchor);

        PlayerPickupCollider.enabled = true;
        ObjectInHand = pickupObject;

        GetComponentInParent<CharacterVocals>()?.PickupObject();
    }
    public void TryDrop()
    {
        if (ObjectInHand == null)
            return;

        ObjectInHand.Drop();

        ObjectInHand = null;
        PlayerPickupCollider.enabled = false;
    }
    public void TryThrowingItem()
    {
        PlayerPickupCollider.enabled = false;

        ObjectInHand.SetValuesToDefault();

        ObjectInHand.ThrowSelf();
        PlayThrowSFX();

        ObjectInHand = null;
    }
    
    private void PlayThrowSFX()
    {
        if (AudioData != null)
        {
            RuntimeManager.PlayOneShot(AudioData.ThrowSFX, transform.position);
            GetComponentInParent<CharacterVocals>()?.ThrowingSpeech();
        }
    }

    public void TryCleanUp()
    {
        ObjectInHand.Cleanup();

        ObjectInHand = null;
        PlayerPickupCollider.enabled = false;
    }
    private GameObject FindClosestInteractable()
    {
        int maxColliders = 3;
        Collider[] hitColliders = new Collider[maxColliders];

        int size = Physics.OverlapBoxNonAlloc(PickupAnchor.position + CharacterData.InteractionOffset, CharacterData.InteractionRange, hitColliders, Quaternion.identity, CharacterData.InteractableMask);

        if (size == 0) 
            return null;

        //Collider nearestCollider = null;
        //foreach (Collider interactable in hitColliders)
        //{
        //    if (ObjectInHand != null && ObjectInHand.Collider == interactable) 
        //        continue;
            
        //    nearestCollider = interactable;
        //    break;
        //}

        Collider nearestCollider = null;
        float shortestDistance = float.MaxValue;
        foreach (Collider interactable in hitColliders)
        {
            if (ObjectInHand != null && ObjectInHand.Collider == interactable)
                continue;

            if (interactable == null) 
                continue;

            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < shortestDistance) 
            {
                nearestCollider = interactable;
                shortestDistance = distance;
            }

        }

        return nearestCollider?.gameObject;
    }

    private Interactor FindClosestInteractor()
    {
        int maxColliders = 3;
        Collider[] hitColliders = new Collider[maxColliders];

        int size = Physics.OverlapBoxNonAlloc(PickupAnchor.position + CharacterData.InteractionOffset, CharacterData.InteractionRange, hitColliders, Quaternion.identity, CharacterData.PlayerLayerMask);

        if (size == 0)
            return null;

        Interactor otherInteractor = null;
        foreach (Collider player in hitColliders)
        {
            if (player != null && player.gameObject.TryGetComponent(out Interactor interactor)) 
            {
                if (interactor == this)
                    continue;

                otherInteractor = interactor;
                break;
            }
        }

        return otherInteractor;
    }
}