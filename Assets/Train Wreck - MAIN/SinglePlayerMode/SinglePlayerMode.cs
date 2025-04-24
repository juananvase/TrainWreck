using GameEvents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerMode : MonoBehaviour
{
    [field: SerializeField] private BoolEventAsset OnSinglePlayerModeActivated { get; set; }
    [SerializeField] private Button _button;
    [SerializeField] private SceneHandler _sceneHandler;

    private void Start()
    {
        _button = GetComponent<Button>();
        _sceneHandler = GetComponent<SceneHandler>();
        _button.onClick.AddListener(ActivateSinglePlayerMode);
    }

    private void ActivateSinglePlayerMode()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _sceneHandler?.LoadNextLevel();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        OnSinglePlayerModeActivated.Invoke(true);
    }
}
