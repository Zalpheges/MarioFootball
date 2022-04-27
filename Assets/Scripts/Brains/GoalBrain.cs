using UnityEngine;

public class GoalBrain : PlayerBrain
{
    private GoalKeeperTree behaviorTree = new GoalKeeperTree();
    private void Start()
    {
        Vector3 InitialGoalPosition = Field.GetGoalKeeperPosition(Player.Team);
        behaviorTree.Setup(Allies, Enemies, Player, InitialGoalPosition);
    }

    public override Action GetAction()
    {
        return behaviorTree.root.Evaluate().Item2;
    }
}
