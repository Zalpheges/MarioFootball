using UnityEngine;

public class Mushroom : ConsumableItem
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Boosting " + player.name);
    }
    protected override void RemoveEffect(Player player)
    {
        Debug.Log("Not boosting " + player.name + " anymore.");
    }
}
