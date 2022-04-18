using UnityEngine;

public class Chomp : PhysicalItem
{
    [SerializeField] private int _nTargets;
    private int _targetCounter = 0;
    private Player _followedPlayer;

    private void Start()
    {
        _followedPlayer = _player.FindEnemyInRange(_player.transform.forward, 60f, false);
    }

    private void SearchForPlayer(Player player)
    {
        _followedPlayer = player.FindMateInRange(_player.transform.forward, 180f, false);
    }

    protected override void Update()
    {
        _direction = (_followedPlayer.transform.position - transform.position).normalized;
        base.Update();
    }
    protected override void ApplyEffect(Player player)
    {
        if (player == _followedPlayer)
        {
            if (++_targetCounter < _nTargets)
                SearchForPlayer(player);
            else
                DestroyItem();
        }

        player.Stun(Player.StunType.Chomped);
    }

    public override void DestroyItem()
    {
        Destroy(gameObject);
    }
}
