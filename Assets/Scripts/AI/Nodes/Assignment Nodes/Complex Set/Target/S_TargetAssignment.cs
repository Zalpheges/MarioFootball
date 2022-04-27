using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

public class S_TargetAssignment : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        int enemyIndex = AssignTarget();
        _root.target = _root.enemyPlayersOrder[enemyIndex];
        return (NodeState.SUCCESS, Action.None);
    }

    private int AssignTarget()
    {
        int enemyIndex = 0;

        for (int index = 0; index < _root.parentTree.Allies.Count; index++, enemyIndex++)
        {
            if (_root.enemyPlayersOrder[enemyIndex] == _root.ballHolder)
                enemyIndex++;
            if (_root.allyPlayersOrder[index] == _root.ballContender || _root.allyPlayersOrder[index].IsPiloted)
                index++;
            if (_root.allyPlayersOrder[index] == _root.player)
                return enemyIndex;
        }

        return 0;
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
