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
        Vector3 playerDirection;
        Vector3 desiredPosition;

        if (!rootInitialized)
        {
            _root = GetRootNode();
            rootInitialized = true;
        }

        if (_root.currentGameState == GameState.attack)
        {
            switch (_root.currentTargetType)
            {
                case TargetType.none:
                    displacement = Field.Ball.transform.position - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    return (NodeState.SUCCESS, Action.Move(displacement.normalized));
                case TargetType.ally:
                    float desiredX = (_root.parentTree.enemyGoalTransform.position + _root.parentTree.playerWithBall.transform.position).x / 2;
                    switch (_root.currentWing)
                    {
                        case Wing.right:
                            desiredPosition = new Vector3(desiredX, 0, Field.HeightOneSixths);
                            displacement = desiredPosition - _root.parentTree.player.transform.position;
                            break;
                        case Wing.left:
                            desiredPosition = new Vector3(desiredX, 0, Field.HeightFiveSixths);
                            displacement = desiredPosition - _root.parentTree.player.transform.position;
                            break;
                        case Wing.center:
                            desiredPosition = new Vector3(desiredX, 0, Field.HeightThreeSixths);
                            displacement = desiredPosition - _root.parentTree.player.transform.position;
                            break;
                        default:
                            displacement = Vector3.zero;
                            break;
                    }
                    return (NodeState.SUCCESS, displacement.magnitude > _root.parentTree.attackThreshold/3 ? Action.Move(displacement.normalized) : Action.None);
                case TargetType.allyWithBall:
                    displacement = _root.parentTree.enemyGoalTransform.position - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    return (NodeState.SUCCESS, Action.Move(displacement.normalized));
            }
        }
        else
        { 
            switch (_root.currentTargetType)
            {
                case TargetType.enemy:
                    playerDirection = _root.target.transform.position - _root.parentTree.playerWithBall.transform.position;
                    playerDirection.Normalize();
                    playerDirection *= _root.parentTree.defenseThreshold;
                    desiredPosition = _root.target.transform.position - playerDirection;
                    displacement = desiredPosition - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    return (NodeState.SUCCESS, displacement.magnitude > _root.parentTree.defenseThreshold/3 
                        ? Action.Move(displacement.normalized) : Action.None);
                case TargetType.enemyWithBall:
                    playerDirection = _root.parentTree.allyGoalTransform.position - _root.parentTree.playerWithBall.transform.position;
                    playerDirection.Normalize();
                    playerDirection *= _root.parentTree.defenseThreshold;
                    desiredPosition = _root.parentTree.playerWithBall.transform.position + playerDirection;
                    displacement = desiredPosition - _root.parentTree.player.transform.position;
                    displacement.y = 0;
                    return (NodeState.SUCCESS, displacement.magnitude > _root.parentTree.defenseThreshold/3  
                        ? Action.Move(displacement.normalized) : Action.None);
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
