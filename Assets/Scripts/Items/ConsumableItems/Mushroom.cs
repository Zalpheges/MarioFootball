using UnityEngine;

public class Mushroom : ConsumableItem
{
    protected override void ApplyEffect(Player player)
    {
        player.StartBoost();
    }

    protected override void RemoveEffect(Player player)
    {
        player.ResetBoost();
    }
}
