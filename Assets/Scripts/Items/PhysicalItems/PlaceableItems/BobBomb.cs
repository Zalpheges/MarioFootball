using UnityEngine;

public class BobBomb : PlaceableItem
{
    [SerializeField] private float _explosionRadius;
    private bool _exploded = false;
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
        else
        {
            Debug.Log("Hit " + player.name + " with banana");
        }
    }
    public override void DestroyItem()
    {
        if (!_exploded)
            ApplyEffect(null);
        Destroy(gameObject);
    }
}
