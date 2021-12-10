using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private Team team;

    protected abstract void OnTriggerEnter(Collider other);
    protected abstract void Move();

}
