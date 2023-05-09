using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingDevicePosition : MonoBehaviour
{
    [SerializeField]
    Transform ARCameraPosition;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        this.transform.position = ARCameraPosition.position;
        this.transform.rotation = ARCameraPosition.rotation;
    }
}
