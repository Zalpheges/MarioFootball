using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class T_GoalUncovered : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector3 BallHolderToGoal = _root.parentTree.enemyGoalTransform.position - _root.player.transform.position;

        foreach(Player enemyPlayer in _root.parentTree.Enemies)
        {
            Vector3 BallHolderToEnemy = enemyPlayer.transform.position - _root.player.transform.position;
            float DotProduct = Vector3.Dot(BallHolderToEnemy.normalized, BallHolderToGoal.normalized);
            Debug.Log(DotProduct);

            if (DotProduct > _root.parentTree.shootAlignmentThreshold)
                return (NodeState.FAILURE, Action.None);
        }

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
