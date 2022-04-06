using UnityEngine;

public class SuperStar : Item
{
    private void Start()
    {
        ApplyEffect(_player);
    }
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Making " + player.name + " a shiny human");
    }
}