using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_MoveGoalKeeper_Support : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float desiredX = 0f;
        float desiredZ = 0f;

        if (Mathf.Abs(_root.ballHolder.transform.position.x) < (Field.Width / 2) - Field.GoalArea.y)
        {
            float offset = Mathf.Sign(_root.goalParentTree.InitialGoalPosition.x) * (Field.Width / 2 - Mathf.Abs(_root.goalParentTree.InitialGoalPosition.x));

            float minX = _root.InitialGoalPosition.x;
            float maxX = _root.InitialGoalPosition.x + (_root.allyTeamSide == TeamSide.West ? Field.GoalArea.y : -Field.GoalArea.y) + offset;
            float t = Mathf.Abs(_root.ballHolder.transform.position.x) / ((Field.Width / 2) - Field.GoalArea.y);

            desiredX = Mathf.Lerp(minX, maxX, t);
            desiredZ = _root.ballHolder.transform.position.z * Field.GoalArea.x / Field.Height;
            //Debug.Log($"{minX}, {maxX}, {t}, {desiredX}");
        }
        else
        {
            if (Mathf.Abs(_root.ballHolder.transform.position.z) < Field.GoalArea.x / 2)
            {
                desiredX = _root.goalParentTree.InitialGoalPosition.x;
                desiredZ = _root.goalParentTree.InitialGoalPosition.z;
            }
            else
            {
                desiredX = _root.ballHolder.transform.position.x;
                desiredZ = _root.ballHolder.transform.position.z * Field.GoalArea.x / Field.Height;
            }
        }

        _root.Position = new Vector3(desiredX, 0, desiredZ);


        _root.actionToPerform = ActionToPerform.Move;
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

