using System.Collections;
using DebugMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static int PreviousLevelBuildIndex = 0;
    private bool isLoading = false;
    
    private void OnEnable()
    {
        DebugMenuSystem.Instance.RegisterObject(this);
    }
    
    private void OnDisable()
    {
        DebugMenuSystem.Instance.DeregisterObject(this);
    }

    public void LoadNextLevel() 
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(SceneLoadWait(nextIndex));
    } 

    public void LoadScene(string sceneName) 
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.LogError("WE ARE TRYING TO LOAD THE SAME SCENE. BAD DEVELOPER(JUAN)!");
            return;
        }
        
        StartCoroutine(SceneLoadWait(sceneName));
    }
    
    
    IEnumerator SceneLoadWait(string sceneName)
    {
        if (isLoading)
        {
            yield break;
        }
        isLoading = true;
        
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncSceneLoad.isDone)
        {
            yield return null;
        }
        
        isLoading = false;
    }
    
    IEnumerator SceneLoadWait(int index)
    {
        if (isLoading)
        {
            yield break;
        }
        isLoading = true;
        
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(index);
        while (!asyncSceneLoad.isDone)
        {
            yield return null;
        }
        
        isLoading = false;
    }

    [DebugCommand("Quit Game")]
    public void Quit() 
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }


    public void UpdatePreviousLevelBuildIndex() 
    {
        PreviousLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadPreviousLevelBuildIndex()
    {
        SceneManager.LoadScene(PreviousLevelBuildIndex);
    }
    
    
    // DEBUG
    #region DEBUG FUNCTIONS

    [DebugCommand("Restart Level")]
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 
    
    [DebugCommand("Main Menu")]
    public void DebugGoBackToMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    [DebugCommand("Load Level 1")]
    public void DebugLoadLevel1() 
    {
        SceneManager.LoadScene("Level-1 Shell test");
    }
    
    [DebugCommand("Load Level 2")]
    public void DebugLoadLevel2() 
    {
        SceneManager.LoadScene("Level-2 Test Shell");
    }
    
    [DebugCommand("Load Level 3")]
    public void DebugLoadLevel3() 
    {
        SceneManager.LoadScene("Level-3 Test Shell");
    }
    
    [DebugCommand("Load Level 4")]
    public void DebugLoadLevel4() 
    {
        SceneManager.LoadScene("Level-4 Test Shell");
    } 
    
    [DebugCommand("Load Win Screen")]
    public void DebugLoadWinScreen() 
    {
        SceneManager.LoadScene("WinScreen");
    }
    
    [DebugCommand("Load Lose Screen")]
    public void DebugLoseScreen() 
    {
        SceneManager.LoadScene("LoseScreen");
    }

    #endregion
    

}
