using UnityEngine;

public abstract class PlaceableItem : PhysicalItem
{
    private bool _stop = false;
    private bool _init = false;
    private const float g = 9.81f;
    private float _stopTimer = 0f;

    private void Init()
    {
        _direction = new Vector3(_direction.x * _data.Speed, _data.Speed, _direction.z * _data.Speed);
    }

    protected override void Update()
    {
        if (_stop)
            return;
        _stopTimer += Time.deltaTime;
        if (_data && !_init)
        {
            Init();
            _init = true;
        }
        _direction -= g * Time.deltaTime * Vector3.up;

        transform.position += _direction * Time.deltaTime;

        if ((transform.position + _direction * Time.deltaTime).y < transform.localScale.y / 2f)
        {
            _stop = _stopTimer >= 1f;
        }
    }
}
