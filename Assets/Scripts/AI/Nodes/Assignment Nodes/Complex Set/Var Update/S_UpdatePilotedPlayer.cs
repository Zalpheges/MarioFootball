using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_UpdatePilotedPlayer : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        foreach (Player allyPlayer in _root.parentTree.Allies)
            if (allyPlayer.IsPiloted)
                _root.pilotedPlayer = allyPlayer;

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

