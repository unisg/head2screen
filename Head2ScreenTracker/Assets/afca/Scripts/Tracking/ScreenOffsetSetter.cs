using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOffsetSetter : MonoBehaviour
{
    private void Start()
    {
        var y = PlayerPrefs.GetInt(EAppSettingsNames.ScreenOffsetY.ToString()) / 1000f;
        this.gameObject.transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
}
