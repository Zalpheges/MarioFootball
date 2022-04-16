using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class T_PositionReached : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float distance = (_root.player.transform.position - _root.CoordinatePosition).magnitude;
        
        if (distance < _root.parentTree.attackThreshold)
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