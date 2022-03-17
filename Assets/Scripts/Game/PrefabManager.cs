using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;

    [SerializeField]
    private GameObject _ball;

    public static GameObject Ball => _instance._ball;

    private void Awake()
    {
        _instance = this;
    }
}
