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
        Debug.Log("Hit " + player.name + " with red turtle shell");
        DestroyItem();
    }
    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
