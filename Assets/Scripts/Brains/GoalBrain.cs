using UnityEngine;

public class GoalBrain : PlayerBrain
{
    private GoalKeeperTree behaviorTree = new GoalKeeperTree();

    // Espace devant le Goal (vers le but ennemi)
    float GoalWidth = Field.GoalArea.y;
    // Espace le long du but (en largeur)
    float GoalHeight = Field.GoalArea.x;

    private void Start()
    {
        Vector3 InitialGoalPosition = Field.GetGoalKeeperPosition(Player.Team);
        behaviorTree.Setup(Allies, Enemies, Player, InitialGoalPosition);
    }

    public override Action GetAction()
    {
        // Position du goal (centre ligne de but)
        Vector3 CenterGoalPosition = Player.Team.transform.position;
        // Position initiale du gardien


        return behaviorTree.root.Evaluate().Item2;
    }
}
