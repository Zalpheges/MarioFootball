using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    private Dictionary<Transform, CinemachineVirtualCamera> _virtualCameras;

    private void Awake()
    {
        _instance = this;
    }

    public static void Init(Transform[] players, Transform ball)
    {
        _instance._virtualCameras = new Dictionary<Transform, CinemachineVirtualCamera>();
        for (int i = 0; i < players.Length + 1; i++)
        {
            Transform t = i < players.Length ? players[i].transform : ball.transform;
            CinemachineVirtualCamera virtualCam =
                Instantiate(PrefabManager.VirtualCamera, _instance.transform).GetComponent<CinemachineVirtualCamera>();
            virtualCam.Follow = t;
            _instance._virtualCameras[t] = virtualCam;
        }
    }

    public static void Follow(Transform toFollow)
    {
        _instance._virtualCameras[toFollow].MoveToTopOfPrioritySubqueue();
    }
}