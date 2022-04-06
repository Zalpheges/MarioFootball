using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StraightTurtleShell : ThrowableItem
{
    [SerializeField] private int _maxRebounds;
    private int _nRebounds = 0;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (++_nRebounds > _maxRebounds)
                Destroy(gameObject);
            else
            {
                Transform fieldT = Field.Transform;
                float verticalDistance = Vector3.Project(transform.position - fieldT.position, fieldT.forward).magnitude;
                float horizontalDistance = Vector3.Project(transform.position - fieldT.position, fieldT.right).magnitude;
                Vector3 normal = (Field.Height / 2f - verticalDistance < Field.Width / 2f - horizontalDistance) ? fieldT.forward : fieldT.right;
                _direction = Vector3.Reflect(_direction, normal);
            }
        }
        base.OnTriggerEnter(other);
    }
}
