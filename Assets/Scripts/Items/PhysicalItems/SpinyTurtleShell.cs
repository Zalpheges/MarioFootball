using UnityEngine;

public class SpinyTurtleShell : PhysicalItem
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
