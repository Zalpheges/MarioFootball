using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class A_Move : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        _root.Position.y = _root.player.transform.position.y;
        return (NodeState.SUCCESS, Action.MoveTo(_root.Position));
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
