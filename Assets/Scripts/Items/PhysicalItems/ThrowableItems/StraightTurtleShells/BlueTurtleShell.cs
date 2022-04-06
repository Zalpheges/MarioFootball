using UnityEngine;

public class BlueTurtleShell : StraightTurtleShell
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Freeze " + player.name);
    }
}
