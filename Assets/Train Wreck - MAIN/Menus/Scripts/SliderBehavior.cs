using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderBehavior : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private AudioSourcesSOData _audioData;

    public void OnDeselect(BaseEventData eventData)
    {
        RuntimeManager.PlayOneShot(_audioData.UIMoveSFX);
    }

    public void PlaySelectSound()
    {
        RuntimeManager.PlayOneShot(_audioData.UISelectSFX);
    }
}
