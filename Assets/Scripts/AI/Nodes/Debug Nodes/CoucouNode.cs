using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CoucouNode : Node
{

    private RootNode _root;
    public override (NodeState, Action) Evaluate()
    {
        _root = GetRootNode();

        Debug.Log(_root.actionToPerform);
        Debug.Log(_root.Position);
        return (NodeState.FAILURE, Action.None);
    }
    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
