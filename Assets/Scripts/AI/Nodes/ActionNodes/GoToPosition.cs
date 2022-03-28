using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class GoToPosition : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        Vector3 displacement;

        if (!rootInitialized)
        {
            _root = GetRootNode();
            rootInitialized = true;
        }

        if (_root.currentGameState == GameState.attack)
        {
            switch (_root.currentBallHolderType)
            {
                case BallHolderType.none:
                    displacement = Field.Ball.transform.position - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    Debug.Log(displacement.normalized);
                    return (NodeState.SUCCESS, Action.Move(displacement.normalized));
                case BallHolderType.ally:
                    return base.Evaluate();
                case BallHolderType.allyWithBall:
                    displacement = _root.parentTree.enemyGoalTransform.position - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    Debug.Log(displacement.normalized);
                    return (NodeState.SUCCESS, Action.Move(displacement.normalized));
            }
        }
        else
        { 
            switch (_root.currentBallHolderType)
            {
                case BallHolderType.enemy:
                    return base.Evaluate();
                case BallHolderType.enemyWithBall:
                    return base.Evaluate();
            }
        }
        return base.Evaluate();
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
