using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableItem : ThrowableItem
{
    [SerializeField] private float _launchDistance;
    private float _traveledDistance = 0f;
    private bool _stop = false;
    protected override void Update()
    {
        Vector3 previousPosition = transform.position;
        if (_stop)
            return;
        else
            Move();
        _traveledDistance += (transform.position - previousPosition).magnitude;
        if (_traveledDistance >= _launchDistance)
            _stop = true;
    }
}
