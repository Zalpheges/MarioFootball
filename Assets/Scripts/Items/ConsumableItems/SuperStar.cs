public class SuperStar : ConsumableItem
{
    protected override void ApplyEffect(Player player)
    {
        player.StartBoost(1.2f, true);
    }

    protected override void RemoveEffect(Player player)
    {
        player.ResetBoost();
    }
}