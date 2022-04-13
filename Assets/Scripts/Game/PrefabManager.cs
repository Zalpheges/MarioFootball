using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;

    [SerializeField]
    private GameObject _ball;

    [SerializeField]
    private GameObject _virtualCamera;

    [SerializeField]
    private List<ItemData> _items = new List<ItemData>();

    public static GameObject Ball => _instance._ball;
    public static GameObject VirtualCamera => _instance._virtualCamera;
    public static List<ItemData> Items => _instance._items;

    //Keep this
    //public struct ItemList
    //{
    //    public GameObject Banana => _instance._banana;
    //    public GameObject BobBomb => _instance._bobBomb;
    //    public GameObject Mushroom => _instance._mushroom;
    //    public GameObject RedTurtleShell => _instance._redTurtleShell;
    //    public GameObject GreenTurtleShell => _instance._greenTurtleShell;
    //    public GameObject BlueTurtleShell => _instance._blueTurtleShell;
    //    public GameObject SpinyTurtleShell => _instance._spinyTurtleShell;
    //    public GameObject Chomp => _instance._chomp;
    //    public GameObject SuperStar => _instance._superStar;
    //}
    //public static readonly ItemList Items;

    private void Awake()
    {
        _instance = this;
    }
}
