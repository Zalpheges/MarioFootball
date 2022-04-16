using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_AssignTargetCoordinates : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    private int ballHolderIndex = new int();

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        ballHolderIndex = GetBallHolderIndex();
        FindOptimalPositionning();

        return (NodeState.SUCCESS, Action.None);
    }

    private void FindOptimalPositionning()
    {
        List<Player> Players = RetrievePlayersToMove();
        List<Vector2Int> PlayersCoords = RetrievePlayersCoordinates();

        List<Coordinate> PlayersWithCoordinates = FusePlayerCoordinate(Players, PlayersCoords);

        List<Coordinate> BestArrangement = FindBestArrangement(PlayersWithCoordinates);
        SetOptimalPositionning(BestArrangement);
    }

    private void SetOptimalPositionning(List<Coordinate> BestArrangement)
    {
        for (int x = 0; x < BestArrangement.Count; x++)
            if (BestArrangement[x].player == _root.player)
                _root.CoordinatePosition = 
                    new Vector3(_root.OptimalCoordinates[x].x * _root.WidthDivision, 0, _root.OptimalCoordinates[x].y * _root.HeightDivision);
    }

    private List<Coordinate> FindBestArrangement(List<Coordinate> PlayerCoords)
    {
        List<Coordinate> ListBuffer = new List<Coordinate>();
        Coordinate Temp;

        List<Coordinate> CurrentBestArrangement = new List<Coordinate>();

        float shortestDistance = float.MaxValue;
        float distance = 0f;

        for (int x = 0; x < 3; x++)
        {
            ListBuffer.Clear();
            ListBuffer.Add(PlayerCoords[x]);
            ListBuffer.AddRange(PlayerCoords);
            ListBuffer.RemoveAt(x + 1);

            distance = ComputeDistance(ListBuffer);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                CurrentBestArrangement.Clear();
                CurrentBestArrangement.AddRange(ListBuffer);
            }

            Temp = ListBuffer[1];
            ListBuffer[1] = ListBuffer[2];
            ListBuffer[2] = Temp;

            distance = ComputeDistance(ListBuffer);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                CurrentBestArrangement.Clear();
                CurrentBestArrangement.AddRange(ListBuffer);
            }
        }

        return CurrentBestArrangement;
    }

    private float ComputeDistance(List<Coordinate> players)
    {
        float distance = 0;

        for (int x = 0; x < players.Count; x++)
            distance += (_root.OptimalCoordinates[x] - players[x].coordinate).magnitude;

        return distance;
    }

    private List<Coordinate> FusePlayerCoordinate(List<Player> players, List<Vector2Int> Coords)
    {
        List<Coordinate> ListToReturn = new List<Coordinate>();

        for (int x = 0; x < players.Count; x++)
        {
            Coordinate coordinate = new Coordinate(players[x], Coords[x]);
            ListToReturn.Add(coordinate);
        }

        return ListToReturn;
    }

    private List<Vector2Int> RetrievePlayersCoordinates()
    {
        List<Vector2Int> CoordsToReturn = new List<Vector2Int>();
        CoordsToReturn.AddRange(_root.PlayersCoordinates);
        CoordsToReturn.RemoveAt(ballHolderIndex);
        return CoordsToReturn;
    }

    private List<Player> RetrievePlayersToMove()
    {
        List<Player> PlayersToReturn = new List<Player>();
        PlayersToReturn.AddRange(_root.parentTree.Allies);
        PlayersToReturn.RemoveAt(ballHolderIndex);
        return PlayersToReturn;
    }

    private int GetBallHolderIndex()
    {
        int index = 0;

        foreach (Player ally in _root.parentTree.Allies)
        {
            if (_root.ballHolder == ally)
                return index;
            index++;
        }

        return index;
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
