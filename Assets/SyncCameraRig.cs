using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCameraRig : MonoBehaviour
{
    public GameObject CameraRig;

    void Start()
    {
    }

    void Update()
    {
        CameraRig.transform.localPosition = transform.localPosition;
    }
}
