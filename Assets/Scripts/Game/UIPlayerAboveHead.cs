using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerAboveHead : MonoBehaviour
{
    private  Transform Cam;

    private void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void LateUpdate()
    {
        transform.LookAt(Cam.transform.position);
    }
}
