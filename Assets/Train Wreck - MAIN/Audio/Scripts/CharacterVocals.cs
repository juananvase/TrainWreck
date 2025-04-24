using CustomFMODFunctions;
using UnityEngine;
using FMODUnity;
using GameEvents;

public class CharacterVocals : MonoBehaviour
{
    [SerializeField] private EventReference ThrowFMODEvent;
    [SerializeField] private EventReference PickupObjectFMODEvent;
    [SerializeField] private EventReference BounceFMODEvent;
    [SerializeField] private EventReference StunnedFMODEvent;
    [SerializeField] private EventReference ItemBounceFMODEvent;
    [SerializeField] private EventReference OnFireVocalsFMODEvent;
    [SerializeField] private EventReference FixingVocalsFMODEvent;

    [SerializeField] private BoolEventAsset OnPlayerIsFixing;
    
    FMOD.Studio.EventInstance _onFireFMODEventInstance;
    FMOD.Studio.EventInstance _fixingFMODEventInstance;

    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        OnPlayerIsFixing.AddListener(StartFixingVocals);
    }

    private void OnDisable()
    {
        OnPlayerIsFixing.RemoveListener(StartFixingVocals);

        if (AudioInstanceHandler.CheckIfPlayingSFX(_fixingFMODEventInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_fixingFMODEventInstance);
        }
        
        if (AudioInstanceHandler.CheckIfPlayingSFX(_onFireFMODEventInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_onFireFMODEventInstance);
        }
    }


    public void ThrowingSpeech()
    {
        if (!ThrowFMODEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(ThrowFMODEvent, transform.position);
        }
    }

    public void PickupObject()
    {
        if (!PickupObjectFMODEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(PickupObjectFMODEvent, transform.position);
        }
    }

    public void Bounce()
    {
        if (!BounceFMODEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(BounceFMODEvent, transform.position);
        }
    }

    public void Stunned()
    {
        if (!StunnedFMODEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(StunnedFMODEvent, transform.position);
        }
    }

    public void ItemBounce()
    {
        if (!ItemBounceFMODEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(ItemBounceFMODEvent, transform.position);
        }
    }

    public void StartOnFireVocals()
    {
        if (!OnFireVocalsFMODEvent.IsNull)
        {
            _onFireFMODEventInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_onFireFMODEventInstance, OnFireVocalsFMODEvent);
            RuntimeManager.AttachInstanceToGameObject(_onFireFMODEventInstance, gameObject, _rb);
        }
    }
    

    public void StopOnFireVocals()
    {
        AudioInstanceHandler.StopAndReleaseSFXInstance(_onFireFMODEventInstance);
    }

    private void StartFixingVocals(bool isFixing)
    {
        if (!FixingVocalsFMODEvent.IsNull)
        {
            if (isFixing)
            {
                if (AudioInstanceHandler.CheckIfPlayingSFX(_fixingFMODEventInstance) == false)
                {
                    _fixingFMODEventInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_fixingFMODEventInstance, FixingVocalsFMODEvent);
                    _fixingFMODEventInstance.set3DAttributes(gameObject.To3DAttributes());
                }
            }
            else
            {
                AudioInstanceHandler.StopAndReleaseSFXInstance(_fixingFMODEventInstance);
            }
            
        }
    }
}
