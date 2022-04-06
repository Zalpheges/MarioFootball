using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "MarioFootball/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite Sprite;

    public GameObject Prefab;

    [Range(1f, 10f)]
    public float Speed;

    public float[] Angles;

    public bool TeamHasToLose;
}
