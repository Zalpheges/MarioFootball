using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableItem : PhysicalItem
{
    private bool _stop = false;
    private bool _init = false;
    private const float g = 9.81f;

    private void Init()
    {
        _direction = new Vector3(_direction.x * _data.Speed, _data.Speed, _direction.z * _data.Speed);
    }

    protected override void Update()
    {
        if (_stop)
            return;
        if (_data && !_init)
        {
            Init();
            _init = true;
        }
        _direction -= g * Time.deltaTime * Vector3.up;

        transform.position += _direction * Time.deltaTime;

        if ((transform.position + _direction * Time.deltaTime).y < transform.localScale.y / 2f)
            _stop = true;
    }
}
