using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Team : MonoBehaviour
{
    [SerializeField]
    private string _ateamBrainType;

    public Type TeamBrainType => Type.GetType(_ateamBrainType);

    [SerializeField]
    private string _agoalBrainType;

    public Type GoalBrainType => Type.GetType(_agoalBrainType);

    public Player[] Players { get; private set; }
    public PlayerBrain[] Brains { get; private set; }
    public Player GoalKeeper { get; private set; }

    public PlayerBrain Brain { get; private set; }

    private Queue<Sprite> _items;
    private int _itemCapacity = 3;

    private void Awake()
    {
        Brain = GetComponentInChildren<PlayerBrain>();
    }

    private void Update()
    {
        if (Field.Team2 == this)
            return;

        Player piloted = null;
        Player hasBall = null;

        for (int i = 0; i < Players.Length; ++i)
        {
            if (Players[i].IsPiloted)
                piloted = Players[i];

            if (Players[i].HasBall)
                hasBall = Players[i];
        }

        if (Brain.Player != piloted)
            Brain.Player = piloted;

        if (hasBall && hasBall != piloted)
        {
            if (Brain.Player)
                Brain.Player.IsPiloted = false;

            Brain.Player = hasBall;

            Brain.Player.IsPiloted = true;
            CameraManager.Follow(Brain.Player.transform);
        }
    }

    /// <summary>
    /// Add an item to the team's item queue, only if it's not full already
    /// </summary>
    public void GainItem()
    {
        if (_items.Count < _itemCapacity)
            return;
        //Keep this
        //var itemProperties = PrefabManager.Items.GetType().GetProperties();
        //int i = UnityEngine.Random.Range(0, itemProperties.Length - 1);
        //_items.Enqueue((itemProperties[i].GetValue(PrefabManager.Items) as GameObject).GetComponent<Item>());
        PrefabManager.Item itemType = (PrefabManager.Item)UnityEngine.Random.Range(0, PrefabManager.ItemSprites.Count);
        Sprite itemSprite = PrefabManager.ItemSprites[itemType];
        _items.Enqueue(itemSprite);
        UIManager.UpdateItems(_items, this);
    }

    /// <summary>
    /// Delete the oldest item from the team item queue
    /// </summary>
    /// <returns>The gameObject corresponding to the deleted item</returns>
    public GameObject GetItem()
    {
        if(_items.Count == 0) 
            return null;
        GameObject itemGo = PrefabManager.ItemPrefabs[_items.Dequeue()];
        UIManager.UpdateItems(_items, this);
        return itemGo;
    }

    /// <summary>
    /// Initialise les joueurs et la file d'items de l'�quipe
    /// </summary>
    /// <param name="players">Les joueurs sans le gardien</param>
    /// <param name="goal">Le gardien</param>
    public void Init(Player[] players, Player goalKeeper)
    {
        Players = players;
        GoalKeeper = goalKeeper;

        _items = new Queue<Sprite>(_itemCapacity);

        Brains = Players.Select(player => player.IABrain).ToArray();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ball>())
        {
            Debug.Log("but");
            GameManager.GoalScored(this);
        }
    }
}
