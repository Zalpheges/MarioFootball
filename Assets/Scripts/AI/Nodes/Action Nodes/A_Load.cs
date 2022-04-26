using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class A_Load : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        _root.TimeLoad += Time.deltaTime;

        return (NodeState.SUCCESS, Action.Loading(_root.TimeLoad / 1.5f));
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
