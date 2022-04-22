using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    [SerializeField] private ParticleSystem _effectPS;
    [SerializeField] protected float _duration;
    protected float _timer;

    protected void Start()
    {
        ApplyEffect(_player);
        _effectPS.Play();
        transform.SetParent(_player.transform, true);
        transform.localPosition = Vector3.zero;
    }

    protected void Update()
    {
        if ((_timer += Time.deltaTime) <= _duration)
            return;
        RemoveEffect(_player);
        DestroyItem();
    }

    protected abstract void RemoveEffect(Player player);
}
