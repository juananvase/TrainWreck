using System;
using System.Collections;
using System.Security.Cryptography;
using EditorTools;
using FMODUnity;
using GameEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Pickupable : PooledObject, IPickupable
{
    //Basic build in unity components
    [field: SerializeField, FoldoutGroup("Components"), AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField, FoldoutGroup("Components"), AutoAssign(AutoAssignAttribute.AutoAssignMode.Parent)] public Collider Collider { get; private set; }

    //Important data and Game events and SOs
    [field: SerializeField, FoldoutGroup("Data"), InlineEditor] public PickupableDataSO PickupableData { get; private set; }
    [field: SerializeField, FoldoutGroup("Data")] public CharacterDataSO CharacterData { get; private set; }
    [field: SerializeField, FoldoutGroup("Data")] protected AudioSourcesSOData AudioData { get; private set; }
    
    [SerializeField, FoldoutGroup("Data")] protected PickupablesEventAsset _onUpdateSpawnedResourcesList;
    [field: SerializeField] public bool IsBeingHeld { get; protected set; } = false;
    
    [Header("Throw Trail")]
    [SerializeField] private GameObject _throwTrailPrefab;
    [SerializeField] private VisualEffect _throwTrailVFX;
    

    private bool _respawned; 
    private bool _isThrowTrailVFXPlaying;

    public virtual void Interact(Interactor interactor)
    {
        //
    }

    private void Start()
    {
        _throwTrailPrefab.SetActive(false);
        _throwTrailVFX?.Stop();
    }

    public virtual void Pickup(Transform pickupAnchor)
    {
        //Check for basic components
        if (Rigidbody == null)
        {
            Debug.LogError("Missing Rigidbody component");
            return;
        }

        if (Collider == null)
        {
            Debug.LogError("Missing Collider component");
            return;
        }

        //Set values to pickup values
        Collider.enabled = false;
        Rigidbody.isKinematic = true;
        IsBeingHeld = true;

        //Place itself on the correct position
        transform.SetParent(pickupAnchor);
        transform.SetPositionAndRotation(pickupAnchor.position, pickupAnchor.rotation);
    }
    public void SetValuesToDefault()
    {
        //Set values to default values
        Collider.enabled = true;
        Rigidbody.isKinematic = false;
        IsBeingHeld = false;

        //detach from parent
        transform.SetParent(null);

        if (_isThrowTrailVFXPlaying)
        {
            _throwTrailVFX?.Stop();
            _throwTrailPrefab.SetActive(false);
            _isThrowTrailVFXPlaying = false;
        }
    }
    public virtual void Drop()
    {
        SetValuesToDefault();
    }

    public virtual void Cleanup()
    {
        //Call game event to update the RS list
        _onUpdateSpawnedResourcesList?.Invoke(this);

        SetValuesToDefault();

        //Return to its resource pool
        ReturnToPool();
    }

    public void ThrowSelf()
    {
        Rigidbody.AddForce(transform.forward * CharacterData.ThrowForce, ForceMode.Impulse);
        // play vfx
        if (_throwTrailVFX != null)
        {
            _isThrowTrailVFXPlaying = true;
            _throwTrailPrefab.SetActive(true);
            _throwTrailVFX.Play();
            StartCoroutine(StopPlayingVFX());
        }
    }

    private IEnumerator StopPlayingVFX()
    {
        yield return new WaitForSeconds(1f);
        _throwTrailVFX.Stop();
        _throwTrailPrefab.SetActive(false);
        _isThrowTrailVFXPlaying = false;
    }

    public void Respawn(Transform targetPos, float speed)
    {
        _respawned = false;
        StartCoroutine(MoveOutOfScreen(targetPos, speed));
    }
    
    private IEnumerator MoveOutOfScreen(Transform targetPos, float speed)
    {
        float elapsedTime = 0;

        while (!_respawned)
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(Rigidbody.transform.position, targetPos.position, speed / elapsedTime);
            Rigidbody.linearVelocity = new Vector3(newPosition.x * (speed * Time.deltaTime), 0f, targetPos.position.z);

            if (Vector3.Distance(Rigidbody.transform.position, targetPos.position) < 1f)
            {
                _respawned = true;    
            }
            
            if (elapsedTime >= CharacterData.MaxRespawnTime)
            {
                Rigidbody.linearVelocity = Vector3.zero;
                Rigidbody.MovePosition(GameManager.Instance.RespawnPoint.position);
                _respawned = true;
                yield break; 
            }
            yield return null;
        }
        Rigidbody.position = GameManager.Instance.RespawnPoint.position;
    }


    private IEnumerator OnCollisionEnter(Collision _)
    {
        if (_isThrowTrailVFXPlaying && _throwTrailVFX != null)
        {
            yield return new WaitForSeconds(0.5f);
            _throwTrailVFX.Stop();
            _throwTrailPrefab.SetActive(false);
            _isThrowTrailVFXPlaying = false;
        }
    }
}