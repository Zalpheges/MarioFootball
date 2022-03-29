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

    /// <summary>
    /// Ajoute un item à la file d'items de l'équipe, dans le cas où celle-ci n'est pas pleine
    /// </summary>
    public void GainItem()
    {

    }

    /// <summary>
    /// Supprime l'item le plus ancien de la file d'items de l'équipe
    /// </summary>
    /// <returns>L'item supprimé</returns>
    public Item GetItem()
    {
        return _items.Dequeue();
    }

    /// <summary>
    /// Initialise les joueurs et la file d'items de l'équipe
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
            GameManager.GoalScored(this);
    }
}
