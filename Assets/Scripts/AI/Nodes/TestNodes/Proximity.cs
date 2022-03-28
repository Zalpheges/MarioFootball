using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Proximity : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    public Proximity() : base() { }
    
    public override (NodeState, Action) Evaluate()
    {
        if(!rootInitialized)
        {
            _root = GetRootNode();
            rootInitialized = true;
        }    

        Player closestPlayer = null;
        float shortestDistance = 0f;

        foreach (Player player in _root.parentTree.Allies)
        {
            float distance = (Field.Ball.transform.position - player.transform.position).magnitude;

            if (closestPlayer == null)
            {
                closestPlayer = player;
                shortestDistance = distance;
            }
            else
            {
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPlayer = player;
                }
            }
        }

        _root.currentGameState = GameState.attack;

        if (closestPlayer == _root.parentTree.player)
        {
            _root.currentBallHolderType = BallHolderType.none;
            return (NodeState.SUCCESS, Action.None);
        }
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
