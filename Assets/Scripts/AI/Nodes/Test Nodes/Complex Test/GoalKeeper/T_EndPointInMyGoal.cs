using BehaviorTree;
using UnityEngine;

public class T_EndPointInMyGoal : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        if (Mathf.Sign(Field.Ball.EndPoint.x) == Mathf.Sign(_root.goalParentTree.allyGoalTransform.position.x))
            return (NodeState.SUCCESS, Action.None);

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
