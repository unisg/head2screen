using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppEvents {

    #region Scene
    #region OpenScene
    public delegate void OpenSceneAction(string sceneName);
    public static event OpenSceneAction OnOpenScene;


    public static void SendOpenScene(string sceneName)
    {
        if (OnOpenScene != null)
            OnOpenScene(sceneName);
    }
    #endregion
    #region CloseScene
    public delegate void CloseSceneAction(string sceneName);
    public static event CloseSceneAction OnCloseScene;


    public static void SendCloseScene(string sceneName)
    {
        if (OnCloseScene != null)
            OnCloseScene(sceneName);
    }
    #endregion
    #endregion

    #region Communication
    #region Open Socket
    public delegate void OpenSocketAction();
    public static event OpenSocketAction OnOpenSocket;


    public static void SendOpenSocket()
    {
        if (OnOpenSocket != null)
            OnOpenSocket();
    }
    #endregion
    #region Close Socket
    public delegate void CloseSocketAction();
    public static event CloseSocketAction OnCloseSocket;


    public static void SendCloseSocket()
    {
        if (OnCloseSocket != null)
            OnCloseSocket();
    }
    #endregion
    #endregion
}
