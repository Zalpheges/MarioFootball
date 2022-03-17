using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Team Team { get; set; }

    protected abstract void Move();
    protected abstract void OnTriggerEnter(Collider other);
}
