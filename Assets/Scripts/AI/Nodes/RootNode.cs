using BehaviorTree;
using System.Collections.Generic;

public enum GameState
{
    unassigned,
    attack,
    defend
}

public enum TargetType
{
    unassigned,
    none,
    enemy,
    enemyWithBall,
    ally,
    allyWithBall
}

public enum Wing
{
    unassigned,
    left,
    right,
    center
}

public class RootNode : Node
{
    public TreeV2 parentTree;

    public Player target = null;
    public TargetType currentTargetType = TargetType.unassigned;

    public GameState currentGameState = GameState.unassigned;

    public Wing currentWing = Wing.unassigned;

    public RootNode(TreeV2 iparentTree, List<Node> ichildren)
    {
        parentTree = iparentTree;

        foreach (Node child in ichildren)
            _Attach(child);
    }

    public override (NodeState, Action) Evaluate()
    {
        if( this.Children != null )
            return Children[0].Evaluate();
        
        return base.Evaluate();
    }
}
