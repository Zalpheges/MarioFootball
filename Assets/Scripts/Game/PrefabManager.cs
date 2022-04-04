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

    [Header("ItemPrefabs"), Tooltip("The item prefabs to spawn in the game")]
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
    
    [Header("ItemSprites"), Tooltip("The item sprites for UI displaying")]
    [SerializeField]
    private Sprite _bananaSprite;
    [SerializeField]
    private Sprite _bobBombSprite;
    [SerializeField]
    private Sprite _mushroomSprite;
    [SerializeField]
    private Sprite _redTurtleShellSprite;
    [SerializeField]
    private Sprite _greenTurtleShellSprite;
    [SerializeField]
    private Sprite _blueTurtleShellSprite;
    [SerializeField]
    private Sprite _spinyTurtleShellSprite;
    [SerializeField]
    private Sprite _chompSprite;
    [SerializeField]
    private Sprite _superStarSprite;

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
    //public struct ItemSpriteList
    //{
    //    public Sprite BananaSprite => _instance._bananaSprite;
    //    public Sprite BobBombSprite => _instance._bobBombSprite;
    //    public Sprite MushroomSprite => _instance._mushroomSprite;
    //    public Sprite RedTurtleShellSprite => _instance._redTurtleShellSprite;
    //    public Sprite GreenTurtleShellSprite => _instance._greenTurtleShellSprite;
    //    public Sprite BlueTurtleShellSprite => _instance._blueTurtleShellSprite;
    //    public Sprite SpinyTurtleShellSprite => _instance._spinyTurtleShellSprite;
    //    public Sprite ChompSprite => _instance._chompSprite;
    //    public Sprite SuperStarSprite => _instance._superStarSprite;
    //}
    //public static readonly ItemList Items;
    //public static readonly ItemSpriteList ItemSprites;

    public static Dictionary<Item, Sprite> ItemSprites = new Dictionary<Item, Sprite>();
    public static Dictionary<Sprite, GameObject> ItemPrefabs = new Dictionary<Sprite, GameObject>();


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        ItemSprites.Add(Item.Banana, _instance._bananaSprite);
        ItemSprites.Add(Item.BobBomb, _instance._bobBombSprite);
        ItemSprites.Add(Item.Mushroom, _instance._mushroomSprite);
        ItemSprites.Add(Item.RedTurtleShell, _instance._redTurtleShellSprite);
        ItemSprites.Add(Item.GreenTurtleShell, _instance._greenTurtleShellSprite);
        ItemSprites.Add(Item.BlueTurtleShell, _instance._blueTurtleShellSprite);
        ItemSprites.Add(Item.SpinyTurtleShell, _instance._spinyTurtleShellSprite);
        ItemSprites.Add(Item.Chomp, _instance._chompSprite);
        ItemSprites.Add(Item.SuperStar, _instance._superStarSprite);

        ItemPrefabs.Add(_instance._bananaSprite, _instance._banana);
        ItemPrefabs.Add(_instance._bobBombSprite, _instance._bobBomb);
        ItemPrefabs.Add(_instance._mushroomSprite, _instance._mushroom);
        ItemPrefabs.Add(_instance._redTurtleShellSprite, _instance._redTurtleShell);
        ItemPrefabs.Add(_instance._greenTurtleShellSprite, _instance._greenTurtleShell);
        ItemPrefabs.Add(_instance._blueTurtleShellSprite, _instance._blueTurtleShell);
        ItemPrefabs.Add(_instance._spinyTurtleShellSprite, _instance._spinyTurtleShell);
        ItemPrefabs.Add(_instance._chompSprite, _instance._chomp);
        ItemPrefabs.Add(_instance._superStarSprite, _instance._superStar);
    }
}
