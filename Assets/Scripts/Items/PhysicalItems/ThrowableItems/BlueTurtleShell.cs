using UnityEngine;

public class BlueTurtleShell : ThrowableItem
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
