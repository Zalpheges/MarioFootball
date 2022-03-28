using BehaviorTree;
using System.Collections.Generic;

public enum GameState
{
    unassigned,
    attack,
    defend
}

public enum BallHolderType
{
    unassigned,
    none,
    enemy,
    enemyWithBall,
    ally,
    allyWithBall
}

public class RootNode : Node
{
    public TreeV2 parentTree;

    public Player target = null;
    public BallHolderType currentBallHolderType = BallHolderType.unassigned;

    public GameState currentGameState = GameState.unassigned;

    public RootNode(TreeV2 iparentTree, List<Node> ichildren)
    {
        parent = null;
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
