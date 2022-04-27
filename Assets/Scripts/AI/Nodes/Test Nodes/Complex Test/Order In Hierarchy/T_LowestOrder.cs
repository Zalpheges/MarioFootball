using BehaviorTree;

public class T_LowestOrder : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        if (_root.allyPlayersOrder[0] == _root.player)
            return (NodeState.SUCCESS, Action.None);
        if (_root.allyPlayersOrder[0] == _root.ballHolder && _root.allyPlayersOrder[1] == _root.player)
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