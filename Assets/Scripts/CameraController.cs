using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    private Transform _follow = null;

    public Transform ToFollow => _follow;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        _follow ??= _vCam.Follow;
        if(_vCam == CameraManager.ActiveCam?.GetComponent<CinemachineVirtualCamera>())
            _vCam.Follow = CameraManager.LockerDynamic;
    }
}
