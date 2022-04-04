using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class T_BallHolder_ThirdQuarter : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        if (_root.ballHolder.transform.position.x < (Field.Width / 4) && _root.ballHolder.transform.position.x > 0)
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
