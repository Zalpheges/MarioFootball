using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_UpdateBallHolder : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if(!_rootInitialized)
            _root = GetRootNode();

        foreach (Player player in _root.parentTree.Allies)
            if (player.HasBall)
            {
                _root.ballHolder = player;
                return (NodeState.SUCCESS, Action.None);
            }

        foreach (Player player in _root.parentTree.Enemies)
            if (player.HasBall)
            {
                _root.ballHolder = player;
                return (NodeState.SUCCESS, Action.None);
            }

        _root.ballHolder = null;
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
