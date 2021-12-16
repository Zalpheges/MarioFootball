using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field instance;

    private float width;
    private float height;
    [SerializeField] private Team team1, team2;

    private Vector3 bottomLeftCorner;
    private Vector3 bottomRightCorner;
    private Vector3 topLeftCorner;
    private Vector3 topRightCorner;

    private GameObject ball;

    public static Vector3 BottomLeftCorner => instance.bottomLeftCorner;
    public static Vector3 BottomRightCorner => instance.bottomRightCorner;
    public static Vector3 TopLeftCorner => instance.topLeftCorner;
    public static Vector3 TopRightCorner => instance.topRightCorner;
    public static Team Team1 => instance.team1;
    public static Team Team2 => instance.team2;
    public static GameObject Ball => instance.ball;

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

    public static void KickOff()
    {
        instance.ball = Instantiate(PrefabManager.instance.Ball, instance.transform.position, Quaternion.identity);
        
    }
}
