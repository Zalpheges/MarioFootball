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

    public int Score { get; private set; }
    public PlayerBrain Brain { get; private set; }

    private Queue<Item> items;
    private int itemCapacity = 3;

    public void GainItem()
    {

    }

    public Item GetItem()
    {
        return items.Dequeue();
    }

    public void Init(Player[] players, Player goal)
    {
        Players = players;
        Goal = goal;

        items = new Queue<Item>(itemCapacity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ++Score;
    }
}
