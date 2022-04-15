using UnityEngine;

public class BobBomb : PlaceableItem
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionDelay;
    private float _timer = 0f;
    private bool _exploded = false;

    protected override void Update()
    {
        if ((_timer += Time.deltaTime) > _explosionDelay)
            DestroyItem();
        base.Update();
    }

    protected override void ApplyEffect(Player player)
    {
        if(!_exploded)
        {
            _exploded = true;
            foreach(Collider collider in Physics.OverlapSphere(transform.position, _explosionRadius))
            {
                Player p = collider.GetComponent<Player>();
                if(p)
                {
                    ApplyEffect(p);
                }
                else
                    collider.GetComponent<Item>()?.DestroyItem();
            }
            DestroyItem();
        }

        player.Fall((player.transform.position - transform.position).normalized, 6f, 1.5f, 2f);
    }
    public override void DestroyItem()
    {
        if (!_exploded)
            ApplyEffect(null);
        Destroy(gameObject);
    }
}
