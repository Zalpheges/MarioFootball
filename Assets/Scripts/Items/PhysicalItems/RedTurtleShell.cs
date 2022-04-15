using UnityEngine;

public class RedTurtleShell : PhysicalItem
{
    private Player _followedPlayer;

    private void Start()
    {
        _followedPlayer = _player.FindEnemyInRange(_player.transform.forward, 60f, false);
    }

    protected override void Update()
    {
        if(_followedPlayer)
            _direction = (_followedPlayer.transform.position - transform.position).normalized;
        base.Update();
    }

    protected override void ApplyEffect(Player player)
    {
        player.Fall((player.transform.position - transform.position).normalized);
        DestroyItem();
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
