using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadTriangle : MonoBehaviour
{
    private GameObject Cam;
    public GameObject Triangle;
    public Player _Player;
    void Start()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");

    }

    void LateUpdate()
    {
        if (_Player.IsPiloted)
        {
            Triangle.SetActive(true);
            Triangle.transform.LookAt(Cam.transform.position);

        }
        else
        {
            Triangle.SetActive(false);
        }
    }
}


