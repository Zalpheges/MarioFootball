using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_SetContender : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        GetClosestPlayerToAllyGoal();

        return (NodeState.SUCCESS, Action.None);
    }

    private void GetClosestPlayerToAllyGoal()
    {
        Player closestPlayer = null;
        float shortestDistance = 0f;

        foreach (Player player in _root.parentTree.Allies)
        {
            float distanceToGoal = (_root.parentTree.allyGoalTransform.position - player.transform.position).magnitude;

            if (closestPlayer == null)
            {
                closestPlayer = player;
                shortestDistance = distanceToGoal;
            }
            else if (distanceToGoal < shortestDistance)
            {
                closestPlayer = player;
                shortestDistance = distanceToGoal;
            }
        }

        _root.ballContender = closestPlayer;
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

