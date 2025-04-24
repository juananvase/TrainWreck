using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public class PickupableContainerAudioHandler : MonoBehaviour
{

    [SerializeField] private EventReference _fullContainerEnterEvent;
    [SerializeField] private EventReference _fullContainerStayEvent;
    [SerializeField] private EventReference _emptyContainerEnterEvent;
    [SerializeField] private EventReference _emptyContainerStayEvent;

    [SerializeField] private Vector2 _enterMinMaxVelocity = new Vector2(0.5f, 15f);
    [SerializeField] private Vector2 _stayMinMaxVelocity = new Vector2(0.5f, 15f);
    [SerializeField] private float _repeatTime = 0.33f;

    private float _nextPlayTime = 0f;

    public bool _isFull = false;

    private void Start()
    {
        _nextPlayTime = Time.time + _repeatTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isFull)
        {
            PlaySound(_fullContainerEnterEvent, collision.relativeVelocity, _enterMinMaxVelocity);
        }
        else
        {
            PlaySound(_emptyContainerEnterEvent, collision.relativeVelocity, _enterMinMaxVelocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFull)
        {
            PlaySound(_fullContainerEnterEvent, collision.relativeVelocity, _enterMinMaxVelocity);
        }
        else
        {
            PlaySound(_emptyContainerEnterEvent, collision.relativeVelocity, _enterMinMaxVelocity);
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
            if (_isFull)
            {
                PlaySound(_fullContainerStayEvent, collision.relativeVelocity, _enterMinMaxVelocity);
            }
            else
            {
                PlaySound(_emptyContainerStayEvent, collision.relativeVelocity, _enterMinMaxVelocity);
            }
    }

    private void OnCollisionStay(Collision collision)
    {
            if (_isFull)
            {
                PlaySound(_fullContainerStayEvent, collision.relativeVelocity, _enterMinMaxVelocity);
            }
            else
            {
                PlaySound(_emptyContainerStayEvent, collision.relativeVelocity, _enterMinMaxVelocity);
            }
    }

    private void PlaySound(EventReference eventReference, Vector3 relativeVelocity, Vector2 minMaxVelocity)
    {
        //Check to see if enough time has elapsed since the last collision
        if (Time.time <= _nextPlayTime || eventReference.IsNull) return;
        _nextPlayTime = Time.time + _repeatTime;

        //Check to see if the magnitude is greater than the minimum value
        float magnitude = relativeVelocity.magnitude;
        if (magnitude > minMaxVelocity.x)
        {
            //Convert the magnitude to a value betten 0 and 1
            float normalizedMagnitude = Mathf.Clamp01(Mathf.InverseLerp(minMaxVelocity.x, minMaxVelocity.y, magnitude));

            //Create a FMOD event instance and set parameters
            EventInstance instance = RuntimeManager.CreateInstance(eventReference);
            instance.setParameterByName("CollisionVelocity", normalizedMagnitude);
            instance.set3DAttributes(gameObject.To3DAttributes());
            //Start the FMOD event and let it cleanup when finished playing
            instance.start();
            instance.release();
        }
        
    }

}
