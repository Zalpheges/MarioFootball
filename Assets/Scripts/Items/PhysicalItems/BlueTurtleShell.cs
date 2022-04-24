using UnityEngine;

public class BlueTurtleShell : PhysicalItem
{
    protected override void ApplyEffect(Player player)
    {
        if (player == player.Team.Goalkeeper || player.IsWaiting || player.IsNavDriven)
            return;
        base.ApplyEffect(player);
        player.Stun(Player.StunType.Frozen);
    }
}
