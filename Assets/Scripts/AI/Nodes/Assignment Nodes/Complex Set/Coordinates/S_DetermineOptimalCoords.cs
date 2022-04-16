using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_DetermineOptimalCoords : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        int SideModifier = _root.allyTeamSide == TeamSide.West ? 1 : -1;
        Vector2Int Coords = _root.BallHolderCoordinates;
        _root.OptimalCoordinates.Clear();

        if (Mathf.Abs(Coords.x) + Mathf.Abs(Coords.y) == 3) //Corner
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x / 2, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x / 2, 0));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, 0));
        }
        else if (Coords.y == 0)
        {
            if (Mathf.Abs(Coords.x) == 2) //GoalRange
            {
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y + 1));
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y - 1));
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x/2, 0));
            }
            else //Center
            {
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - SideModifier, Coords.y));
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + SideModifier, Coords.y + 1));
                _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + SideModifier, Coords.y - 1));
            }
        }
        else //Side
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + 1, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - 1, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, 0));
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
