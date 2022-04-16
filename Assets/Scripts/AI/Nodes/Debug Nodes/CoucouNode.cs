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

        Debug.Log("BH C" + _root.BallHolderCoordinates);
        foreach(Vector2Int V2 in _root.OptimalCoordinates)
            Debug.Log(V2.ToString());

        return (NodeState.SUCCESS, Action.None);
    }
    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
