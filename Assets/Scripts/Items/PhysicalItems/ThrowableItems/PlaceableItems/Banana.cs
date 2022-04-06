using UnityEngine;

public class Banana : PlaceableItem
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Hit " + player.name + " with banana");
    }
}
