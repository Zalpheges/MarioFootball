using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field instance;

    [SerializeField]
    private float _width;

    [SerializeField]
    private float _height;

    [SerializeField]
    private Team _team1, _team2;

    [SerializeField]
    private Vector2 _attackPosCaptain;

    [SerializeField]
    private Vector2 _attackPosMate1;

    [SerializeField]
    private Vector2 _attackPosMate2;

    [SerializeField]
    private Vector2 _attackPosMate3;

    [SerializeField]
    private Vector2 _defPosCaptain;

    [SerializeField]
    private Vector2 _defPosMate1;

    [SerializeField]
    private Vector2 _defPosMate2;

    [SerializeField]
    private Vector2 _defPosMate3;

    public static Team Team1 => instance._team1;
    public static Team Team2 => instance._team2;

    private Vector3 _bottomLeftCorner;
    public static Vector3 BottomLeftCorner => instance._bottomLeftCorner;

    private Vector3 _bottomRightCorner;
    public static Vector3 BottomRightCorner => instance._bottomRightCorner;

    private Vector3 _topLeftCorner;
    public static Vector3 TopLeftCorner => instance._topLeftCorner;

    private Vector3 _topRightCorner;
    public static Vector3 TopRightCorner => instance._topRightCorner;

    private float _heightOneThird;
    public static float HeightOneThird => instance._heightOneThird;

    private float _heightTwoThirds;
    public static float HeightTwoThirds => instance._heightTwoThirds;

    private float _heightOneSixths;
    public static float HeightOneSixths => instance._heightOneSixths;

    private float _heightThreeSixths;
    public static float HeightThreeSixths => instance._heightThreeSixths;

    private float _heightFiveSixths;
    public static float HeightFiveSixths => instance._heightFiveSixths;

    private Ball _ball;
    public static Ball Ball => instance._ball;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _bottomLeftCorner = new Vector3(_height / 2, 0, -_width / 2) + transform.position;
        _bottomRightCorner = new Vector3(_height / 2, 0, _width / 2) + transform.position;
        _topLeftCorner = new Vector3(-_height / 2, 0, -_width / 2) + transform.position;
        _topRightCorner = new Vector3(-_height / 2, 0, _width / 2) + transform.position;

        _heightOneThird = _topLeftCorner.x + _height / 3f;
        _heightTwoThirds = _topLeftCorner.x + _height * 2f / 3f;

        _heightOneSixths = _topLeftCorner.x + _height / 6f;
        _heightThreeSixths = _topLeftCorner.x + _height * 3f / 6f;
        _heightFiveSixths = _topLeftCorner.x + _height * 5f / 6f;

        GameManager.BreedMePlease(_team1, _team2);
    }

    /// <summary>
    /// Assigne le ballon créé puis le positionne ainsi que les joueurs
    /// </summary>
    /// <param name="ball">Le ballon</param>
    public static void Init(Ball ball)
    {
        instance._ball = ball;

        ball.transform.position = instance.transform.position;

        instance.SetTeamPosition();
    }

    private void SetTeamPosition()
    {
        Team1.Players[0].transform.position = VectorToPosition(_attackPosCaptain);
        Team1.Players[1].transform.position = VectorToPosition(_attackPosMate1);
        Team1.Players[2].transform.position = VectorToPosition(_attackPosMate2);
        Team1.Players[3].transform.position = VectorToPosition(_attackPosMate3);

        Team2.Players[0].transform.position = VectorToPosition(-_defPosCaptain);
        Team2.Players[1].transform.position = VectorToPosition(-_defPosMate1);
        Team2.Players[2].transform.position = VectorToPosition(-_defPosMate2);
        Team2.Players[3].transform.position = VectorToPosition(-_defPosMate3);
    }

    private Vector3 VectorToPosition(Vector2 vector)
    {
        return transform.position + new Vector3(vector.x * _height / 2f, 1f, vector.y * _width / 2f);
    }
}
