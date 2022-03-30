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

    private Queue<Item> _items;
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
        }
    }

    /// <summary>
    /// Ajoute un item � la file d'items de l'�quipe, dans le cas o� celle-ci n'est pas pleine
    /// </summary>
    public void GainItem()
    {

    }

    /// <summary>
    /// Supprime l'item le plus ancien de la file d'items de l'�quipe
    /// </summary>
    /// <returns>L'item supprim�</returns>
    public Item GetItem()
    {
        return _items.Dequeue();
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

        _items = new Queue<Item>(_itemCapacity);

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
