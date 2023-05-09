using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestTracking : MonoBehaviour
{
    int maxX ;
    int maxY ;

    private void OnEnable()
    {
        AppEvents.SendOpenSocket();
    }
    private void OnDisable()
    {
        AppEvents.SendCloseSocket();
    }

    public void SendCorners()
    {
        loadResolution();
        SendTracking.Instance.SendPosition(0, 0);
        Thread.Sleep(1000);
        SendTracking.Instance.SendPosition(maxX-1, 0);
        Thread.Sleep(1000); 
        SendTracking.Instance.SendPosition(0, maxY-1);
        Thread.Sleep(1000);
        SendTracking.Instance.SendPosition(maxX-1, maxY-1);
        Thread.Sleep(1000);

    }

    public void MoveLeftRight()
    {
        loadResolution();
        int halfY = maxY / 2;
        for(int i = 0; i<maxX/100; i++)
        {
            SendTracking.Instance.SendPosition(i*100, halfY);
            Thread.Sleep(6000);
        }
       

    }

    private void loadResolution()
    {
        maxX = PlayerPrefs.GetInt(EAppSettingsNames.ScreenSizeX.ToString());
        maxY = PlayerPrefs.GetInt(EAppSettingsNames.ScreenSizeY.ToString());

    }
}
