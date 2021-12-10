using System.Collections.Generic;
using UnityEngine;
using System;

public class Team : MonoBehaviour
{
    private Player[] players;

    public int score;
    public PlayerBrain Brain { get; private set; }

    private Queue<Item> items;
    private int itemCapacity = 3;

    private void Awake()
    {
        items = new Queue<Item>(itemCapacity);
    }

    public void GainItem()
    {

    }

    public Item GetItem()
    {
        return items.Dequeue();
    }

    public void Init(Player[] players)
    {
        this.players = players;
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
