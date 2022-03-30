using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    private Dictionary<Transform, CinemachineVirtualCamera> _virtualCameras;
    [SerializeField] private Transform _lockerLeft;
    [SerializeField] private Transform _lockerRight;
    [SerializeField] private Transform _lockerDynamic;

    public static Transform LockerLeft => _instance._lockerLeft;
    public static Transform LockerRight => _instance._lockerRight;
    public static Transform LockerDynamic => _instance._lockerDynamic;
    public static GameObject ActiveCam => CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Vector3 activeFollowPosition = ActiveCam.GetComponent<CameraController>().ToFollow.position;
        float z = Mathf.Max(_lockerLeft.position.z, Mathf.Min(activeFollowPosition.z, _lockerRight.position.z));

        _lockerDynamic.position = new Vector3(activeFollowPosition.x, activeFollowPosition.y, z);
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