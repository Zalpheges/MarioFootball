using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private BasicTree _basicTree;

    public Action actionToPerform;

    private void Start()
    {
        _basicTree = this.gameObject.AddComponent<BasicTree>();
    }

    private void Update()
    {
        _basicTree.UpdateVars(Field.Ball.transform, transform, Player.HasBall);
    }

    public override Action GetAction()
    {
        if (actionToPerform == null && !Player.HasBall)
            return Action.Move(Vector3.zero);
        return actionToPerform;
    }
}
