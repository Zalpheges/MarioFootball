public class RedTurtleShell : PhysicalItem
{
    private Player _followedPlayer;

    private void Start()
    {
        _followedPlayer = _player.FindEnemyInRange(_player.transform.forward, 60f, includeGoalKeeper: false);
    }

    protected override void Update()
    {
        if (_followedPlayer)
            _direction = (_followedPlayer.transform.position - transform.position).normalized;
        base.Update();
    }

    protected override void ApplyEffect(Player player)
    {
        if (player == player.Team.Goalkeeper || player.IsWaiting || player.IsNavDriven)
        {
            DestroyItem();
            return;
        }
        base.ApplyEffect(player);
        player.Fall((player.transform.position - transform.position).normalized);
        DestroyItem();
    }
}
