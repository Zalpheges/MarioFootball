using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_AssignTargetCoordinates_Goal : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

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
        Coordinate FirstBuffer;
        Coordinate Temp;

        List<Coordinate> CurrentBestArrangement = new List<Coordinate>();

        float shortestDistance = float.MaxValue;
        float distance = 0f;

        for (int y = 0; y < 4; y++)
        {
            FirstBuffer = PlayerCoords[y];

            for (int x = 0; x < 4; x++)
            {
                if (y == 3)
                    break;

                if (x == y)
                    x++;

                ListBuffer.Clear();
                ListBuffer.Add(FirstBuffer);
                ListBuffer.Add(PlayerCoords[x]);
                foreach (Coordinate c in PlayerCoords)
                    if (!ListBuffer.Contains(c))
                        ListBuffer.Add(c);

                distance = ComputeDistance(ListBuffer);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    CurrentBestArrangement.Clear();
                    CurrentBestArrangement.AddRange(ListBuffer);
                }

                Temp = ListBuffer[2];
                ListBuffer[2] = ListBuffer[3];
                ListBuffer[3] = Temp;

                distance = ComputeDistance(ListBuffer);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    CurrentBestArrangement.Clear();
                    CurrentBestArrangement.AddRange(ListBuffer);
                }
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
        return CoordsToReturn;
    }

    private List<Player> RetrievePlayersToMove()
    {
        List<Player> PlayersToReturn = new List<Player>();
        PlayersToReturn.AddRange(_root.parentTree.Allies);
        return PlayersToReturn;
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
