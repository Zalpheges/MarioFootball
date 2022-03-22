using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private BasicTree _basicTree;

    public Action actionToPerform;

    private void Start()
    {
        _basicTree = this.gameObject.AddComponent<BasicTree>();
        _basicTree.Initialize(Field.Ball.transform, this.gameObject.transform, this.Player.HasBall);
    }

    public override Action GetAction()
    {
        if (actionToPerform == null && !Player.HasBall)
            return Action.Move(Vector3.zero);
        return actionToPerform;
    }
}
