using BehaviorTree;
using UnityEngine;

public class S_UpdateBallSeekerCoordinates : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        _root.PreviousBallSeekerCoordinates = _root.BallSeekerCoordinates;
        _root.BallSeekerCoordinates = GetCoordinates(_root.ballSeeker);

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
