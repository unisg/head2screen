using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;
using UnityEngine.XR.ARSubsystems;

public class RayCast : MonoBehaviour
{
    [SerializeField]
    ARFaceManager faceManager;

    [SerializeField]
    GameObject testFace;

    int screenX;
    int screenY;

    private void OnEnable()
    {
        SendTracking.Instance.InitSocket();
    }
    private void OnDisable()
    {
        SendTracking.Instance.CloseSocket();
    }
    private void Start()
    {
        screenX = PlayerPrefs.GetInt(EAppSettingsNames.ScreenSizeX.ToString());
        screenY = PlayerPrefs.GetInt(EAppSettingsNames.ScreenSizeY.ToString());
    }

    void Update()
    {
       
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //Debug 
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(testFace.transform.position, testFace.transform.TransformDirection(Vector3.back), out hit))
            {
                Debug.DrawRay(testFace.transform.position, hit.point, Color.yellow);
                Debug.Log("Did Hit");
            }
            
        } else
        {
            if (faceManager.trackables.count == 1)
            {
                foreach (var face in faceManager.trackables)
                {
                    RaycastHit hit;
                    //    Debug.Log(face.transform.position);
                    // Does the ray intersect any objects excluding the player layer
                    var fp = face.transform.position;
                    fp.z *= -1;
                    if (Physics.Raycast(fp, face.transform.TransformDirection(Vector3.forward), out hit))
                    {
                        int x = (int)(hit.point.x * 1000) + (screenX / 2);
                        int y = (int)(hit.point.y * 1000) + (screenY / 2);
                        SendTracking.Instance.SendPosition(x,y);
                    }

                }
            }
        }
        
            
        
       
    }
}
