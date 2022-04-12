using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_AssignSeeker : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Player closestPlayer = null;
        float shortestDistance = 0f;

        foreach(Player ally in _root.parentTree.Allies)
        {
            float distance = (Field.Ball.transform.position - ally.transform.position).magnitude;

            if (closestPlayer == null || distance < shortestDistance)
            {
                Debug.Log(closestPlayer);
                Debug.Log(distance);
                Debug.Log(shortestDistance);
                closestPlayer = ally;
                shortestDistance = distance;
            }
        }

        _root.ballSeeker = closestPlayer;

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

