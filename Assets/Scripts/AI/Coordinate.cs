using UnityEngine;

public class Coordinate
{
    public Player player;
    public Vector2Int coordinate;

    public Coordinate(Player iPlayer, Vector2Int iCoordinate)
    {
        player = iPlayer;
        coordinate = iCoordinate;
    }
}
