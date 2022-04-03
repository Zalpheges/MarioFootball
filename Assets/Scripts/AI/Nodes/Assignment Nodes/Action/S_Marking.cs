using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_Marking : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector3 positionToMark = _root.target.transform.position +
            (_root.ballHolder.transform.position - _root.target.transform.position).normalized * _root.parentTree.markThreshold;

        if ((positionToMark - _root.player.transform.position).magnitude < _root.parentTree.defenseThreshold)
        {
            _root.actionToPerform = ActionToPerform.None;
            return (NodeState.SUCCESS, Action.None);
        }

        _root.actionToPerform = ActionToPerform.Move;
        _root.Position = positionToMark;
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
