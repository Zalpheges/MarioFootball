using UnityEngine;

public class GreenTurtleShell : ThrowableItem
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Push " + player.name);
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
