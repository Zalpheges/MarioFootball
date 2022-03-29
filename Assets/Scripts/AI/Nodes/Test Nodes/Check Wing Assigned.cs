using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckWingAssigned : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!rootInitialized)
            _root = GetRootNode();

        if(_root.currentWing != Wing.unassigned)
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
