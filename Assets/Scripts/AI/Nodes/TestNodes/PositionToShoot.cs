using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class PositionToShoot : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if(!rootInitialized)
            _root = GetRootNode();

        float distanceToGoal = (_root.parentTree.enemyGoalTransform.position - _root.parentTree.player.transform.position).magnitude;

        if (distanceToGoal <= _root.parentTree.shootThreshold)
            return (NodeState.SUCCESS, Action.None);

        return base.Evaluate();
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
