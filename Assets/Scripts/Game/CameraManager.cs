using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static GameObject ActiveCam => CinemachineCore.Instance?.GetActiveBrain(0)?.ActiveVirtualCamera?.VirtualCameraGameObject;

    #region Lockers

    [SerializeField] private Transform _lockerLeft;
    [SerializeField] private Transform _lockerRight;
    [SerializeField] private Transform _lockerDown;
    [SerializeField] private Transform _lockerDynamic;

    public static Transform LockerLeft => _instance._lockerLeft;
    public static Transform LockerRight => _instance._lockerRight;
    public static Transform LockerDown => _instance._lockerDown;
    public static Transform LockerDynamic => _instance._lockerDynamic;

    #endregion

    #region Camera Queue

    public struct CameraControlQueue
    {
        private Queue<Transform> _transformsToFollow;
        private Queue<float> _cameraFocusDurations;

        private void Init()
        {
            _transformsToFollow = new Queue<Transform>();
            _cameraFocusDurations = new Queue<float>();
        }

        public void AddCameraControl(Transform playerT, float duration)
        {
            if (_transformsToFollow == null)
                Init();
            _transformsToFollow.Enqueue(playerT);
            _cameraFocusDurations.Enqueue(duration);
        }

        public (Transform, float) GetNext()
        {
            if (_transformsToFollow.Count < 1)
                return (null, 0f);
            return (_transformsToFollow.Dequeue(), _cameraFocusDurations.Dequeue());
        }
    }

    public static CameraControlQueue CamerasQueue;

    private bool _processQueue = false;
    private float _timer;
    private float _currentTimeLimit;
    private Transform _nextTransformToFollow;

    #endregion

    private Dictionary<Transform, CinemachineVirtualCamera[]> _virtualCameras;

    private CinemachineVirtualCamera _currentTopCamera;

    #region Public functions
    public static void ReadQueue()
    {
        if (_instance._processQueue)
            return;
        _instance._processQueue = true;
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        (_instance._nextTransformToFollow, _instance._currentTimeLimit) = CamerasQueue.GetNext();
    }

    public static void Init(Transform[] players, Transform ball)
    {
        _instance._virtualCameras = new Dictionary<Transform, CinemachineVirtualCamera[]>();
        for (int i = 0; i < players.Length + 1; i++)
        {
            Transform t = i < players.Length ? players[i].transform : ball.transform;
            CinemachineVirtualCamera virtualCamTop =
                Instantiate(PrefabManager.VirtualCameraTop, _instance.transform).GetComponent<CinemachineVirtualCamera>();
            virtualCamTop.Follow = t;

            CinemachineVirtualCamera virtualCamOrbital =
                Instantiate(PrefabManager.VirtualCameraOrbital, _instance.transform).GetComponent<CinemachineVirtualCamera>();
            virtualCamOrbital.Follow = virtualCamOrbital.LookAt = t;

            _instance._virtualCameras[t] = new CinemachineVirtualCamera[]
            {
                virtualCamTop, 
                virtualCamOrbital
            };
        }
    }

    public static void Follow(Transform toFollow)
    {
        _instance._currentTopCamera = _instance._virtualCameras[toFollow][0];
        if (!_instance._processQueue)
            _instance._currentTopCamera.MoveToTopOfPrioritySubqueue();
    }

    public static void LookAt(Transform toLookAt)
    {
        _instance._virtualCameras[toLookAt][1].MoveToTopOfPrioritySubqueue();
    }

    #endregion

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Vector3 activeFollowPosition = ActiveCam?.GetComponent<CameraController>().ToFollow.position ?? Vector3.zero;

        float x = Mathf.Max(_lockerLeft.position.x, Mathf.Min(activeFollowPosition.x, _lockerRight.position.x));

        float z = Mathf.Max(_lockerDown.position.z, activeFollowPosition.z);

        _lockerDynamic.position = new Vector3(x, activeFollowPosition.y, z);

        if (_processQueue)
            UpdateQueue();

        #region Local functions
        void UpdateQueue()
        {
            if ((_timer += Time.deltaTime) > _currentTimeLimit)
            {
                _timer = 0f;
                if(_nextTransformToFollow == null)
                {
                    EndQueueProcess();
                }
                else
                {
                    LookAt(_nextTransformToFollow);
                    (_nextTransformToFollow, _currentTimeLimit) = CamerasQueue.GetNext();
                }
            }

            void EndQueueProcess()
            {
                _processQueue = false;
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                _instance._currentTopCamera.MoveToTopOfPrioritySubqueue();
            }
        }
        #endregion
    }
}