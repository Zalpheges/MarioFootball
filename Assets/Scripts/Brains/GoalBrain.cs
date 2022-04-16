using UnityEngine;

public class GoalBrain : PlayerBrain
{
    public override Action GetAction()
    {
        // Position du goal (centre ligne de but)
        // Player.Team.transform.position

        // Espace devant le Goal (vers le but ennemi)
        // Field.GoalArea.y

        // Espace le long du but (en largeur)
        // Field.GoalArea.x

        // Position initiale du gardien
        // Field.GetGoalKeeperPosition(Player.Team)

        return Action.MoveTo(Field.GetGoalKeeperPosition(Player.Team));
    }
}
