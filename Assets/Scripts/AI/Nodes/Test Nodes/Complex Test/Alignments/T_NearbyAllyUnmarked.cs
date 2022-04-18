using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class T_NearbyAllyUnmarked : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        foreach (Player allyPlayer in _root.Allies)
        {
            if (allyPlayer != _root.player)
            {
                Vector3 BallHolderToAlly = allyPlayer.transform.position - _root.player.transform.position;

                foreach (Player enemyPlayer in _root.Enemies)
                {
                    bool allyMarked = false;

                    Vector3 BallHolderToEnemy = enemyPlayer.transform.position - _root.player.transform.position;

                    if (Vector3.Dot(BallHolderToAlly.normalized, BallHolderToEnemy.normalized) > _root.passAlignmentThreshold)
                    {
                        if(BallHolderToAlly.sqrMagnitude > BallHolderToEnemy.sqrMagnitude) 
                            allyMarked = true;
                        break;
                    }

                    if (!allyMarked)
                    {
                        _root.passTarget = allyPlayer;
                        return (NodeState.SUCCESS, Action.None);
                    }
                }
            }
        }

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
