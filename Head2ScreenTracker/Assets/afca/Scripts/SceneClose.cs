using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneClose : MonoBehaviour
{
    public void CloseScene(string name)
    {
        AppEvents.SendCloseScene(name);
    }
}
