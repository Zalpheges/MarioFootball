using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;

    [SerializeField]
    private GameObject _ball;

    [SerializeField]
    private GameObject _virtualCameraOrbital;

    [SerializeField]
    private GameObject _virtualCameraTop;

    [SerializeField]
    private List<ItemData> _items = new List<ItemData>();

    public static GameObject Ball => _instance._ball;
    public static GameObject VirtualCameraTop => _instance._virtualCameraTop;
    public static GameObject VirtualCameraOrbital => _instance._virtualCameraOrbital;
    public static List<ItemData> Items => _instance._items;

    private void Awake()
    {
        _instance = this;
    }
}
