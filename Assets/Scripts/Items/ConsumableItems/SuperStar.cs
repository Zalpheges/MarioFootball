using UnityEngine;

public class SuperStar : ConsumableItem
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Making " + player.name + " a shiny human");
    }
    protected override void RemoveEffect(Player player)
    {
        Debug.Log(player.name + " isn't shiny anymore.");
    }
}