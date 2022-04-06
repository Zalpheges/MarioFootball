using UnityEngine;

public class Mushroom : Item
{
    private void Start()
    {
        ApplyEffect(_player);
    }
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Boosting " + player.name);
    }
}
