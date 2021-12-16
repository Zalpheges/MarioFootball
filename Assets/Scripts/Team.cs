using System.Collections.Generic;
using UnityEngine;
using System;

public class Team : MonoBehaviour
{
    public string _ateamBrainType;
    public Type TeamBrainType => Type.GetType(_ateamBrainType);

    public string _agoalBrainType;
    public Type GoalBrainType => Type.GetType(_agoalBrainType);

    public Player[] Players { get; private set; }
    public Player Goal { get; private set; }

    public int ConcededGoals { get; private set; }
    public PlayerBrain Brain { get; private set; }

    private Queue<Item> items;
    private int itemCapacity = 3;

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
        return items.Dequeue();
    }

    /// <summary>
    /// Initialise les joueurs et la file d'items de l'équipe
    /// </summary>
    /// <param name="players">Les joueurs sans le gardien</param>
    /// <param name="goal">Le gardien</param>
    public void Init(Player[] players, Player goal)
    {
        Players = players;
        Goal = goal;

        items = new Queue<Item>(itemCapacity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ++ConcededGoals;
    }
}
