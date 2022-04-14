using UnityEngine;

public class BlueTurtleShell : PhysicalItem
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
