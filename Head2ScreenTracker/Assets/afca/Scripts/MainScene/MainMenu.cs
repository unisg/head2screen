using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MenuLoadScene(string sceneName)
    {
        AppEvents.SendOpenScene(sceneName);
    }
}
