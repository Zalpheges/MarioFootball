using BehaviorTree;
using UnityEngine;

public class S_MoveBallHolderUp : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector3 UpWardPosition = _root.player.transform.position + new Vector3(0, 0, 1f);
        _root.actionToPerform = ActionToPerform.Move;
        _root.Position = UpWardPosition;

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
