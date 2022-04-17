using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadTriangle : MonoBehaviour
{
    public GameObject Cam;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        transform.LookAt( new Vector3(Cam.transform.position.x, Cam.transform.position.y, Cam.transform.position.z )  );
    }
}
