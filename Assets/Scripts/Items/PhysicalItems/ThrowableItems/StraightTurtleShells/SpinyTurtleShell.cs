using UnityEngine;

public class SpinyTurtleShell : StraightTurtleShell
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Bully " + player.name);
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
