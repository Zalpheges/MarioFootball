using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowableItem : PhysicalItem
{
    protected virtual void Update()
    {
        Move();
    }
    protected void Move()
    {
        transform.position += _data.Speed * Time.deltaTime * _direction;
    }
}
