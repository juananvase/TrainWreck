using CustomFMODFunctions;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterWinLoseAudio : MonoBehaviour
{
    [SerializeField] private EventReference WinEvent;
    [SerializeField] private EventReference LoseEvent;

    private EventInstance _winEventInstance;
    private EventInstance _loseEventInstance;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (AudioInstanceHandler.CheckIfPlayingSFX(_winEventInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_winEventInstance);
        }
        if (AudioInstanceHandler.CheckIfPlayingSFX(_loseEventInstance))
        {
            AudioInstanceHandler.StopAndReleaseSFXInstance(_loseEventInstance);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "WinScreen")
        {
            _winEventInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_winEventInstance, WinEvent);
            _winEventInstance.set3DAttributes(gameObject.To3DAttributes());
        }
        else if (scene.name == "LoseScreen")
        {
            _loseEventInstance = AudioInstanceHandler.CreateAndPlaySFXInstance(_loseEventInstance, LoseEvent);
            _loseEventInstance.set3DAttributes(gameObject.To3DAttributes());
        }
    }
}
