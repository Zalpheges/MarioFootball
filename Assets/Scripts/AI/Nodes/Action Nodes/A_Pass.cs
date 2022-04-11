using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class A_Pass : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector3 passDirection = _root.passTarget.transform.position - _root.player.transform.position;
        return (NodeState.SUCCESS, Action.Pass(passDirection));
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