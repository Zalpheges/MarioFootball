using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_MoveGoalKeeper_Defend : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        if (Mathf.Abs(Field.Ball.transform.position.x) > (Field.Width/2) - Field.GoalArea.y && 
                                                        Mathf.Abs(Field.Ball.transform.position.z) < Field.GoalArea.x/2)
        {
            if ((_root.ballHolder.transform.position - _root.player.transform.position).magnitude < _root.headButtThreshold)
            {
                _root.actionToPerform = ActionToPerform.HeadButt;
                _root.target = _root.ballHolder;
            }
            else
            {
                _root.actionToPerform = ActionToPerform.Move;
                _root.Position = Field.Ball.transform.position;
            }    
        }
        else
        {
            float desiredX = 0f;
            float desiredZ = 0f;

            float offset = Mathf.Sign(_root.goalParentTree.InitialGoalPosition.x) * (Field.Width / 2 - Mathf.Abs(_root.goalParentTree.InitialGoalPosition.x));

            float maxX = _root.InitialGoalPosition.x;
            float minX = _root.InitialGoalPosition.x + (_root.allyTeamSide == TeamSide.West ? Field.GoalArea.y : -Field.GoalArea.y) + offset;
            float t = Mathf.Abs(Field.Ball.transform.position.x) / ((Field.Width / 2) - Field.GoalArea.y);

            desiredX = Mathf.Lerp(minX, maxX, t);
            desiredZ = Field.Ball.transform.position.z * (Field.GoalArea.x / Field.Height) * (1 - Mathf.Abs(Field.Ball.transform.position.x) / (2 * Field.Width / 3));

            _root.actionToPerform = ActionToPerform.Move;
            _root.Position = new Vector3(desiredX, 0, desiredZ);
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
