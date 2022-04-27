using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<Transform, CinemachineVirtualCamera[]> _virtualCameras = new Dictionary<Transform, CinemachineVirtualCamera[]>();

    private CinemachineVirtualCamera _currentTopCamera;

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

        public void AddCameraFocus(Transform playerT, float duration)
        {
            if (_transformsToFollow == null)
                Init();
            _transformsToFollow.Enqueue(playerT);
            _cameraFocusDurations.Enqueue(duration);
        }

        public float GetNext()
        {
            if (_transformsToFollow.Count < 1)
                return 0f;
            LookAt(_transformsToFollow.Dequeue());
            return _cameraFocusDurations.Dequeue();
        }

        public void Clear()
        {
            _transformsToFollow.Clear();
            _cameraFocusDurations.Clear();
        }
    }

    public static CameraControlQueue CamerasQueue;

    private bool _processQueue = false;
    private float _timer;
    private float _currentTimeLimit;

    public static void ReadQueue()
    {
        if (_instance._processQueue)
            return;
        _instance._processQueue = true;
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        _instance._currentTimeLimit = CamerasQueue.GetNext();
    }

    public static void SkipQueue()
    {
        CamerasQueue.Clear();
        _instance._currentTimeLimit = 0f;
    }

    #endregion

    public static void Init(Transform[] players, Transform ball)
    {
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

    #region Choose Camera

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

    #region Awake/Update

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
                if (_currentTimeLimit == 0f)
                {
                    EndQueueProcess();
                }
                else
                {
                    _currentTimeLimit = CamerasQueue.GetNext();
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

    #endregion

}