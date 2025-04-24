using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Task : MonoBehaviour, IInteractable
{
    [field: SerializeField, InlineEditor, FoldoutGroup("Data")] public TaskDataSO TaskData { get; private set; }

    //Input
    protected float interactInput;
    protected bool canInteract = false;

    protected virtual void OnEnable()
    {
        TaskData.OnInteractionInput.AddListener(ReceivingInput);
    }
    protected virtual void OnDisable()
    {
        TaskData.OnInteractionInput.RemoveListener(ReceivingInput);
    }

    public virtual void Interact(Interactor interactor) 
    {
        //Check if the input value is greater than 0 to check if it is holding the interaction button
        canInteract = (interactInput > 0.5f);
    }
    protected virtual void OnGameLost(bool isGameLost) 
    {
        //In case something needs to happen when the game is lost
    }

    protected void ReceivingInput(float value)
    {
        interactInput = value;
    }

    protected virtual void PlaySFX(EventReference SFX)
    {
        RuntimeManager.PlayOneShot(SFX, transform.position);
    }

    protected void StopSFX(FMOD.Studio.EventInstance SFX)
    {
        if (!IsPlayingSFX(SFX)) 
            return;

        SFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SFX.release();
    }

    protected bool IsPlayingSFX(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

}