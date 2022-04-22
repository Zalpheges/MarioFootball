using UnityEngine;

public class GreenTurtleShell : PhysicalItem
{
    protected override void ApplyEffect(Player player)
    {
        if (player == player.Team.Goalkeeper || player.IsWaiting || player.IsNavDriven)
            return;
        base.ApplyEffect(player);
        player.Fall((player.transform.position - transform.position).normalized);
    }
}
