using System.Collections.Generic;
using UnityEngine;
using System;

public class Team : MonoBehaviour
{
    private Player[] players;

    public int score;

    private Queue<Item> items;
    private int maxItemCount = 3;

    private void Awake()
    {
        items = new Queue<Item>(maxItemCount);
    }

    public void GainItem()
    {

    }

    public Item UseItem()
    {
        return items.Dequeue();
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    public bool IsTeammate(Player player)
    {
        return Array.Find(players, teammate => teammate.Equals(player));
    }
}
