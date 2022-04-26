using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class A_Shoot : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float random = Random.Range(0.0f, _root.randomMaxValue);
        if (random < 1f)
            return (NodeState.SUCCESS, Action.Shoot(1f));
        _root.actionToPerform = ActionToPerform.Load;
        return (NodeState.SUCCESS, Action.Stop());
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
