using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;



public class AppSceneController : MonoBehaviour
{
    private void OnEnable()
    {
        LoadScene("StartScene");
        RegisterEventHandler();
    }
    private void OnDisable()
    {
        UnRegisterEventHandler();
    }

    #region Event Handlers
    private void RegisterEventHandler()
    {
        AppEvents.OnOpenScene += AppEvents_OnOpenScene;
        AppEvents.OnCloseScene += AppEvents_OnCloseScene;
    }
    private void UnRegisterEventHandler()
    {
        AppEvents.OnOpenScene -= AppEvents_OnOpenScene;
        AppEvents.OnCloseScene -= AppEvents_OnCloseScene;
    }

    private async void AppEvents_OnOpenScene(string sceneName)
    {
        await this.LoadScene(sceneName);
    }

    private async void AppEvents_OnCloseScene(string sceneName)
    {
        await this.CloseScene(sceneName);
    }
    #endregion

    #region Open / Close Scenes
    private async Task LoadScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.name != "MainScene") SceneManager.UnloadSceneAsync(s);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    private async Task CloseScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);
    }
    #endregion
}
