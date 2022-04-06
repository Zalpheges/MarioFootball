using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicalItem : Item
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        Item item = other.GetComponent<Item>();
        if (player)
        {
            if (player.Team == _team)
                Debug.Log("Copain");
            else
            {
                Debug.Log("Act");
                ApplyEffect(player);
            }
        }
        if (item)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
