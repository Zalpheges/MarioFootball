using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolyBrain : PlayerBrain
{
    public enum Placement
    {
        Unassigned,
        RightWing,
        LeftWing,
        Center
    }

    private Placement placement;

    public override Vector2 Move(Team team)
    {
        foreach (Player player in team.Players)
        {
            HolyBrain brain = player.IABrain as HolyBrain;


        }

        return Vector2.zero;
    }
}
