using BehaviorTree;
using UnityEngine;

public class S_MoveBallHolderToGoal : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector3 DisplacementToGoal = (_root.parentTree.enemyGoalTransform.position - _root.player.transform.position).normalized;

        Vector3 ForwardPosition = _root.player.transform.position + DisplacementToGoal;
        _root.actionToPerform = ActionToPerform.Move;
        _root.Position = ForwardPosition;

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
