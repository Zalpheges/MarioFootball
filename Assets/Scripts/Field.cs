using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field instance;

    [SerializeField] private float width;
    [SerializeField] private float height;

    [SerializeField] private Team team1, team2;

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

    // TODO Bryan : Récupérer positions initiales

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

        Team1.Players[0].transform.position = ball.transform.position;

        // TODO Bryan : Positionner les joueurs
    }
}
