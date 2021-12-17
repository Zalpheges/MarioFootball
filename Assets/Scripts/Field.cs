using System.Collections;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field instance;

    [SerializeField] private float width;
    [SerializeField] private float height;

    [SerializeField] private Team team1, team2;

    [SerializeField] private Vector2 attackPosCaptain;
    [SerializeField] private Vector2 attackPosMate1;
    [SerializeField] private Vector2 attackPosMate2;
    [SerializeField] private Vector2 attackPosMate3;

    [SerializeField] private Vector2 defPosCaptain;
    [SerializeField] private Vector2 defPosMate1;
    [SerializeField] private Vector2 defPosMate2;
    [SerializeField] private Vector2 defPosMate3;

    public static Team Team1 => instance.team1;
    public static Team Team2 => instance.team2;

    private Vector3 bottomLeftCorner;
    public static Vector3 BottomLeftCorner => instance.bottomLeftCorner;

    private Vector3 bottomRightCorner;
    public static Vector3 BottomRightCorner => instance.bottomRightCorner;

    private Vector3 topLeftCorner;
    public static Vector3 TopLeftCorner => instance.topLeftCorner;

    private Vector3 topRightCorner;
    public static Vector3 TopRightCorner => instance.topRightCorner;

    // TODO Bryan : Rï¿½cupï¿½rer positions initiales

    private Ball ball;
    public static Ball Ball => instance.ball;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bottomLeftCorner = new Vector3(width / 2, 0, height / 2) - transform.position;
        bottomRightCorner = new Vector3(width / 2, 0, -height / 2) + transform.position;
        topLeftCorner = new Vector3(-width / 2, 0, height / 2) + transform.position;
        topRightCorner = new Vector3(width / 2, 0, height / 2) + transform.position;

        GameManager.BreedMePlease(team1, team2);
    }
    /// <summary>
    /// Assigne le ballon créé puis le positionne ainsi que les joueurs
    /// </summary>
    /// <param name="ball">Le ballon</param>
    public static void Init(Ball ball)
    {
        instance.ball = ball;

        ball.transform.position = instance.transform.position;

        instance.SetTeamPosition();

        // TODO Bryan : Positionner les joueurs
    }


    private void SetTeamPosition()
    {
        Team1.Players[0].transform.position = VectorToPosition(attackPosCaptain);
        Team1.Players[0].IsPiloted = true;
        Team1.Players[1].transform.position = VectorToPosition(attackPosMate1);
        Team1.Players[2].transform.position = VectorToPosition(attackPosMate2);
        Team1.Players[3].transform.position = VectorToPosition(attackPosMate3);

        Team2.Players[0].transform.position = VectorToPosition(-defPosCaptain);
        Team2.Players[1].transform.position = VectorToPosition(-defPosMate1);
        Team2.Players[2].transform.position = VectorToPosition(-defPosMate2);
        Team2.Players[3].transform.position = VectorToPosition(-defPosMate3);
    }

    private Vector3 VectorToPosition(Vector2 vector)
    {
        return transform.position + new Vector3(vector.x * height / 2f, 1f, vector.y * width / 2f);
    }

        
}
