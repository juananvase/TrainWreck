using CustomFMODFunctions;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;

public class MusicHandler : MonoBehaviour
{
    [SerializeField] private AudioSourcesSOData _audioData;
    private EventInstance _currentMusicInstance;
    private float _currentMusicValue;
    private static MusicHandler Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        CheckScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        _currentMusicInstance =
            AudioInstanceHandler.CreateAndPlaySFXInstance(_currentMusicInstance, _audioData.GameMusic);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene(scene.buildIndex);
    }

    private void CheckScene(int buildIndex)
    {
        switch (buildIndex)
        {
            case 0 or 1:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 0);
                break;
            case 2:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 1);
                break;
            case 3:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 2);
                break;
            case 4:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 3);
                break;
            case 5:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 4);
                break;
            case 6:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 5);
                break;
            case 7:
                RuntimeManager.StudioSystem.setParameterByName("ChangeMusicState", 6);
                break;
        }
    }
}
