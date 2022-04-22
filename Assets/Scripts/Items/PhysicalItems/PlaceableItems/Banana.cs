using UnityEngine;

public class Banana : PlaceableItem
{
    protected override void ApplyEffect(Player player)
    {
        if (player == player.Team.Goalkeeper || player.IsWaiting || player.IsNavDriven)
            return;
        base.ApplyEffect(player);
        player.Fall(Vector3.zero, 0f, 1f, 2f);
        DestroyItem();
    }
}
