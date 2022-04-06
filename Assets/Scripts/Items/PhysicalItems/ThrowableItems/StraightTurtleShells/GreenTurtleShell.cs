using UnityEngine;

public class GreenTurtleShell : StraightTurtleShell
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Push " + player.name);
    }
}
