using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _vCam;
    private Transform _follow = null;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        _follow ??= _vCam.Follow;
        if (_follow.position.z < CameraManager.LockerLeft.position.z)
        {
            _vCam.Follow = CameraManager.LockerLeft;
        }
        if (_follow.position.z > CameraManager.LockerRight.position.z)
        {
            _vCam.Follow = CameraManager.LockerRight;
        }
        else if (_follow.position.z >= CameraManager.LockerLeft.position.z && _follow.position.z <= CameraManager.LockerRight.position.z)
        {
            _vCam.Follow = _follow;
        }
    }
}
