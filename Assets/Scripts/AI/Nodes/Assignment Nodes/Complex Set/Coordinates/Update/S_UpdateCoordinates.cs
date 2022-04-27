using BehaviorTree;
using UnityEngine;

public class S_UpdateCoordinates : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        int allyIndex = 0;

        foreach (Player ally in _root.parentTree.Allies)
        {
            if (ally == _root.ballHolder)
            {
                if (ally == _root.ballHolder)
                    _root.PreviousBallHolderCoordinates = _root.BallHolderCoordinates;
            }

            _root.PlayersCoordinates[allyIndex] = GetCoordinates(ally);
            allyIndex++;
        }

        return (NodeState.SUCCESS, Action.None);
    }

    private Vector2Int GetCoordinates(Player player)
    {
        bool xCoordDetermined = false;
        bool yCoordDetermined = false;

        Vector2Int Coord = new Vector2Int();

        for (int x = 0; x < Mathf.Max(_root.WidthDivisionAmount, _root.HeightDivisionAmount); x++)
        {
            if (!xCoordDetermined)
            {
                if (player.transform.position.x < _root.WidthDivision * (x + 0.5) &&
                                                player.transform.position.x > _root.WidthDivision * (x - 0.5))
                {
                    Coord.x = x;
                    xCoordDetermined = true;
                }
                else if (player.transform.position.x < -_root.WidthDivision * (x - 0.5) &&
                                                player.transform.position.x > -_root.WidthDivision * (x + 0.5))
                {
                    Coord.x = -x;
                    xCoordDetermined = true;
                }
            }
            if (!yCoordDetermined)
            {
                if (player.transform.position.z < _root.HeightDivision * (x + 0.5) &&
                                                player.transform.position.z > _root.HeightDivision * (x - 0.5))
                {
                    Coord.y = x;
                    yCoordDetermined = true;
                }
                else if (player.transform.position.z < -_root.HeightDivision * (x - 0.5) &&
                                                player.transform.position.z > -_root.HeightDivision * (x + 0.5))
                {
                    Coord.y = -x;
                    yCoordDetermined = true;
                }
            }
        }

        if (player == _root.ballHolder)
            _root.BallHolderCoordinates = Coord;

        if (player == _root.player)
            _root.PlayerCoordinates = Coord;

        return Coord;
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