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

    [Header("Items")]
    [SerializeField]
    private GameObject _banana;
    [SerializeField]
    private GameObject _bobBomb;
    [SerializeField]
    private GameObject _mushroom;
    [SerializeField]
    private GameObject _redTurtleShell;
    [SerializeField]
    private GameObject _greenTurtleShell;
    [SerializeField]
    private GameObject _blueTurtleShell;
    [SerializeField]
    private GameObject _spinyTurtleShell;
    [SerializeField]
    private GameObject _chomp;
    [SerializeField]
    private GameObject _superStar;

    public GameObject Banana => _instance._banana;
    public GameObject BobBomb => _instance._bobBomb;
    public GameObject Mushroom => _instance._mushroom;
    public GameObject RedTurtleShell => _instance._redTurtleShell;
    public GameObject GreenTurtleShell => _instance._greenTurtleShell;
    public GameObject BlueTurtleShell => _instance._blueTurtleShell;
    public GameObject SpinyTurtleShell => _instance._spinyTurtleShell;
    public GameObject Chomp => _instance._chomp;
    public GameObject SuperStar => _instance._superStar;


    public enum Item
    {
        Banana,
        BobBomb,
        Mushroom,
        RedTurtleShell,
        GreenTurtleShell,
        BlueTurtleShell,
        SpinyTurtleShell,
        Chomp,
        SuperStar,
    }

    public static GameObject Ball => _instance._ball;
    public static GameObject VirtualCamera => _instance._virtualCamera;

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

    public static Dictionary<Item, GameObject> Items = new Dictionary<Item, GameObject>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Items.Add(Item.Banana, Banana);
        Items.Add(Item.BobBomb, BobBomb);
        Items.Add(Item.Mushroom, Mushroom);
        Items.Add(Item.RedTurtleShell, RedTurtleShell);
        Items.Add(Item.GreenTurtleShell, GreenTurtleShell);
        Items.Add(Item.BlueTurtleShell, BlueTurtleShell);
        Items.Add(Item.SpinyTurtleShell, SpinyTurtleShell);
        Items.Add(Item.Chomp, Chomp);
        Items.Add(Item.SuperStar, SuperStar);
    }
}
