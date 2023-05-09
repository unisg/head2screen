using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSettings : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnDisable()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
