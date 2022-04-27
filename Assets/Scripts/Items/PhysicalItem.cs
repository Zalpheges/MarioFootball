using UnityEngine;

public abstract class PhysicalItem : Item
{
    [SerializeField] private ParticleSystem _onHitPS;
    [SerializeField] private GameObject _onDestroyPS;
    [SerializeField] private int _maxRebounds;
    private int _nRebounds = 0;

    protected virtual void Update()
    {
        Move();
        transform.LookAt(transform.position + _direction);
    }
    protected void Move()
    {
        transform.position += _data.Speed * Time.deltaTime * _direction;
    }

    protected override void ApplyEffect(Player player)
    {
        _onHitPS.Play();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (++_nRebounds > _maxRebounds)
                DestroyItem();
            else
            {
                Transform fieldT = Field.Transform;
                float verticalDistance = Vector3.Project(transform.position - fieldT.position, fieldT.forward).magnitude;
                float horizontalDistance = Vector3.Project(transform.position - fieldT.position, fieldT.right).magnitude;
                Vector3 normal = (Field.Height / 2f - verticalDistance < Field.Width / 2f - horizontalDistance) ? fieldT.forward : fieldT.right;
                _direction = Vector3.Reflect(_direction, normal);
            }
        }

        Player player = other.GetComponent<Player>();
        Item item = other.GetComponent<Item>();
        if (player)
        {
            ApplyEffect(player);
        }
        if (item)
        {
            if (GetType() != typeof(Chomp))
                DestroyItem();
        }
    }
    private void OnDestroy()
    {
        if (!_isQuitting)
        {
            Destroy(Instantiate(_onDestroyPS, transform.position, Quaternion.identity), 2f);
        }
    }
}
