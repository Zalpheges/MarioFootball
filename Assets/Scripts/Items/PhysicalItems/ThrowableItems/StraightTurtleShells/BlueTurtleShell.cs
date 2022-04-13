using UnityEngine;

public class BlueTurtleShell : StraightTurtleShell
{
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }

    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Freeze " + player.name);
    }
}
