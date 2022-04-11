using UnityEngine;

public class BobBomb : PlaceableItem
{
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Hit " + player.name + " with banana");
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
