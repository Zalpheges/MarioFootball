using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class T_BallHolder_ShootRange : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float DistanceToGoal = (_root.parentTree.enemyGoalTransform.position - _root.player.transform.position).magnitude;

        if (DistanceToGoal < _root.shootThreshold)
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

