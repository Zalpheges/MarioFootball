using UnityEngine;

public class Banana : PlaceableItem
{
    protected override void ApplyEffect(Player player)
    {
        player.Fall(Vector3.zero, 0f, 1f, 2f);
        DestroyItem();
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
