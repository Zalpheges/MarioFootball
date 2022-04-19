using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_DetermineOptimalCoords_Goal : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        int SideModifier = _root.allyTeamSide == TeamSide.West ? 1 : -1;
        _root.OptimalCoordinates.Clear();

        if (_root.parentTree.allyGoalKeeper.HasBall)
        {
            _root.OptimalCoordinates.Add(new Vector2Int(-SideModifier * (_root.WidthDivisionAmount - 1) / 2 + 1, 2));
            _root.OptimalCoordinates.Add(new Vector2Int(-SideModifier * (_root.WidthDivisionAmount - 1) / 2 + 1, -2));
            _root.OptimalCoordinates.Add(new Vector2Int(-SideModifier * (_root.WidthDivisionAmount - 1) / 2 + 2, 1));
            _root.OptimalCoordinates.Add(new Vector2Int(-SideModifier * (_root.WidthDivisionAmount - 1) / 2 + 2, -1));

        }
        else
        {
            _root.OptimalCoordinates.Add(new Vector2Int(0, 2));
            _root.OptimalCoordinates.Add(new Vector2Int(0, -2));
            _root.OptimalCoordinates.Add(new Vector2Int(SideModifier, 1));
            _root.OptimalCoordinates.Add(new Vector2Int(SideModifier, -1));

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
