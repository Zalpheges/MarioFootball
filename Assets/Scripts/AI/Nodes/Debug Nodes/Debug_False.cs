using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Debug_False : Node
{

    private RootNode _root;
    public override (NodeState, Action) Evaluate()
    {
        _root = GetRootNode();

        _root.actionToPerform = ActionToPerform.Move;
        _root.Position = _root.player.transform.position;
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
