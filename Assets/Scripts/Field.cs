using UnityEngine;

public class Field : MonoBehaviour
{
    private int width;
    private int height;
    [SerializeField] private Team team1, team2;

    public Vector3 BottomLeftCorner { get; private set; }
    public Vector3 BottomRightCorner { get; private set; }
    public Vector3 TopLeftCorner { get; private set; }
    public Vector3 TopRightCorner { get; private set; }

    private void Start()
    {
        GameManager.BreedMePlease(team1, team2);
    }
}
