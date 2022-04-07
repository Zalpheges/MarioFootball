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

        Debug.Log(_root.player.transform.GetSiblingIndex());
        Debug.Log(_root.currentPlayerType);
        Debug.Log(_root.ballHolder);
        Debug.Log(_root.ballHolder.transform.position.x);
        Debug.Log(_root.ballHolder.transform.position.z);
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
