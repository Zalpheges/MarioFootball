using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;

    [SerializeField]
    private GameObject _ball;

    [SerializeField]
    private GameObject _virtualCamera;

    public static GameObject Ball => _instance._ball;
    public static GameObject VirtualCamera => _instance._virtualCamera;

    private void Awake()
    {
        _instance = this;
    }
}
