using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected Team _team;
    protected Player _player;
    protected ItemData _data;
    protected Vector3 _direction;
    protected bool _isQuitting = false;
    protected abstract void ApplyEffect(Player player);
    public void Init(ItemData data, Player player, Vector3 direction)
    {
        _player = player;
        _team = player.Team;
        _direction = direction;
        _data = data;
    }
    public virtual void DestroyItem()
    {
        Destroy(gameObject);
    }
    protected void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}