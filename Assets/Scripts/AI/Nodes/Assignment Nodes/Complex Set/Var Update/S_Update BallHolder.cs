using BehaviorTree;

public class S_UpdateBallHolder : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        foreach (Player player in _root.Allies)
            if (player.HasBall)
            {
                _root.previousBallHolder = _root.ballHolder;
                _root.ballHolder = player;
                return (NodeState.SUCCESS, Action.None);
            }

        foreach (Player player in _root.Enemies)
            if (player.HasBall)
            {
                _root.previousBallHolder = _root.ballHolder;
                _root.ballHolder = player;
                return (NodeState.SUCCESS, Action.None);
            }

        _root.previousBallHolder = _root.ballHolder;
        _root.ballHolder = null;
        return (NodeState.SUCCESS, Action.None);
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
