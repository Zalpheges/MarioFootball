using BehaviorTree;

public class T_BallInAllyHalf : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        if (_root.allyTeamSide == TeamSide.West)
            if (Field.Ball.transform.position.x < 0f)
                return (NodeState.SUCCESS, Action.None);
            else
                return (NodeState.FAILURE, Action.None);

        if (Field.Ball.transform.position.x > 0f)
            return (NodeState.SUCCESS, Action.None);

        return (NodeState.FAILURE, Action.None);
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        _rootInitialized = true;

        return (RootNode)currentNode;
    }
}