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

    private Placement placement = Placement.Unassigned;

    public override Action GetAction()
    {
        return Action.Move(Vector3.left);
    }
}
