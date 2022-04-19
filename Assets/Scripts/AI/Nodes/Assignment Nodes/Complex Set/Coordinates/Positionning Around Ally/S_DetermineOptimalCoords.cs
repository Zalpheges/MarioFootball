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

        if (Mathf.Abs(Coords.x) + Mathf.Abs(Coords.y) == (_root.WidthDivisionAmount - 1) / 2 + (_root.HeightDivisionAmount - 1) / 2) //Corner
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - (int)Mathf.Sign(Coords.x), Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - (int)Mathf.Sign(Coords.x), 0));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y - (int)Mathf.Sign(Coords.y)));
        }
        else if (Coords.y == 0 && Mathf.Abs(Coords.x) == (_root.WidthDivisionAmount - 1) / 2) //GoalRange
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y + 1));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y - 1));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - (int)Mathf.Sign(Coords.x), 0));
        }
        else if (Mathf.Abs(Coords.y) == (_root.HeightDivisionAmount - 1)/2) //Side
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + 1, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - 1, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x, Coords.y - (int)Mathf.Sign(Coords.y)));

        }
        else //Center
        {
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x - SideModifier, Coords.y));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + SideModifier, Coords.y + 1));
            _root.OptimalCoordinates.Add(new Vector2Int(Coords.x + SideModifier, Coords.y - 1));
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
