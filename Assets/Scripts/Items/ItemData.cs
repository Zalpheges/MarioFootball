using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "MarioFootball/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite Sprite;

    public GameObject Prefab;

    [Range(10f, 30f)]
    public float Speed;

    public float[] Angles;

    public bool TeamHasToLose;
}
