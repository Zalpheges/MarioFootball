using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_MoveGoalKeeper_Default : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        _root.actionToPerform = ActionToPerform.Move;
        _root.Position = _root.goalParentTree.InitialGoalPosition - Mathf.Sign(_root.goalParentTree.InitialGoalPosition.x) * new Vector3(Field.GoalArea.y/2, 0, 0);

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
