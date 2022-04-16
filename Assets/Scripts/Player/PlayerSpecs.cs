using UnityEngine;

[CreateAssetMenu(fileName = "Specs", menuName = "MarioFootball/PlayerSpecs")]
public class PlayerSpecs : ScriptableObject
{
    public string Name;

    public float Weight;

    [Range(0f, 1f)]
    public float Accuracy = 0.7f;

    [Range(1f, 10f)]
    public float Speed = 4f;

    [Range(0f, 5f)]
    public float StunTime = 1f;

    public Player Prefab;
    public GameObject StaticPrefab;
}
