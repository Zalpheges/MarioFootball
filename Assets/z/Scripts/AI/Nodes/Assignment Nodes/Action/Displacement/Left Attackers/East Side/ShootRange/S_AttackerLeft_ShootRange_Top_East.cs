using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_AttackerLeft_ShootRange_Top_East : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float positionX = _root.ballHolder.transform.position.x + Field.Width / 6;
        float positionZ = _root.ballHolder.transform.position.z;

        _root.Position = new Vector3(positionX, 0, positionZ);
        _root.actionToPerform = ActionToPerform.Move;
        return (NodeState.SUCCESS, Action.None);
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
