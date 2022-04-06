using UnityEngine;

public class RedTurtleShell : ThrowableItem
{
    private Transform _followedPlayer;
    protected override void ApplyEffect(Player player)
    {
        Debug.Log("Hit " + player.name + " with red turtle shell");
    }

    private void Start()
    {
        //_followedPlayer = FindEnemyPlayer();
    }

    protected override void Update()
    {
        base.Update();
        //_direction = followedPlayer.position - transform.position;
    }
}
