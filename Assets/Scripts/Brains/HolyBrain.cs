using UnityEngine;

public class HolyBrain : PlayerBrain
{
    public enum Placement
    {
        Unassigned,
        RightWing,
        LeftWing,
        Center
    }

    public Placement placement = Placement.Unassigned;

    public override Vector3 Move()
    {
        Vector3 position = transform.position;
        Placement placement = Placement.LeftWing;

        if (position.x > Field.HeightOneThird * 2f)
            placement = Placement.RightWing;
        else if (position.x > Field.HeightTwoThirds)
            placement = Placement.Center;

        bool isFree = true;
        
        foreach (HolyBrain brain in Allies.Brains)
        {
            if (IsOther(brain.Player))
            {
                if (placement == brain.placement)
                    isFree = false;
            }
        }

        if (isFree)
            this.placement = placement;
        else
        {
            bool left = false, center = false, right = false;

            foreach (HolyBrain brain in Allies.Brains)
            {
                if (IsOther(brain.Player))
                {
                    if (brain.placement == Placement.LeftWing)
                        left = true;
                    else if (brain.placement == Placement.Center)
                        center = true;
                    else if (brain.placement == Placement.RightWing)
                        right = true;
                }
            }

            foreach (Player player in Allies.Players)
            {
                if (IsOther(player))
                {
                    HolyBrain brain = player.IABrain as HolyBrain;

                    if (placement == brain.placement)
                    {

                    }
                }
            }

            if (!left)
                this.placement = Placement.LeftWing;
            else if (!center)
                this.placement = Placement.Center;
            else if (!right)
                this.placement = Placement.RightWing;
        }

        float capz = 0f;
        foreach (Player player in Allies.Players)
            if (player.IsPiloted)
                capz = player.Position.z;

        float desiredX = Field.HeightOneSixths;

        if (this.placement == Placement.RightWing)
            desiredX = Field.HeightFiveSixths;
        else if (this.placement == Placement.Center)
            desiredX = Field.HeightThreeSixths;

        return new Vector3(desiredX, 1f, capz);
    }
}
