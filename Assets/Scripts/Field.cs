using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field _instance;

    [SerializeField]
    private float _width;
    public static float Width => _instance._width;

    [SerializeField]
    private float _height;
    public static float Height => _instance._height;

    [SerializeField]
    private float _goalWidth;
    public static float GoalWidth => _instance._goalWidth;

    [SerializeField]
    private float _goalHeight;
    public static float GoalHeight => _instance._goalHeight;

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

    public static Team Team1 => _instance._team1;
    public static Team Team2 => _instance._team2;

    private Vector3 _bottomLeftCorner;
    public static Vector3 BottomLeftCorner => _instance._bottomLeftCorner;

    private Vector3 _bottomRightCorner;
    public static Vector3 BottomRightCorner => _instance._bottomRightCorner;

    private Vector3 _topLeftCorner;
    public static Vector3 TopLeftCorner => _instance._topLeftCorner;

    private Vector3 _topRightCorner;
    public static Vector3 TopRightCorner => _instance._topRightCorner;

    private float _heightOneThird;
    public static float HeightOneThird => _instance._heightOneThird;

    private float _heightTwoThirds;
    public static float HeightTwoThirds => _instance._heightTwoThirds;

    private float _heightOneSixths;
    public static float HeightOneSixths => _instance._heightOneSixths;

    private float _heightThreeSixths;
    public static float HeightThreeSixths => _instance._heightThreeSixths;

    private float _heightFiveSixths;
    public static float HeightFiveSixths => _instance._heightFiveSixths;

    private Ball _ball;
    public static Ball Ball => _instance._ball;

    public static Transform Transform => _instance.transform;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {

        _bottomLeftCorner = transform.TransformPoint(new Vector3(-_width / 2f, 0f, -_height / 2f));
        _bottomRightCorner = transform.TransformPoint(new Vector3(_width / 2f, 0f, -_height / 2f));
        _topLeftCorner = transform.TransformPoint(new Vector3(-_width / 2f, 0f, _height / 2f));
        _topRightCorner = transform.TransformPoint(new Vector3(_width / 2f, 0f, _height / 2f));

        _heightOneThird = _bottomLeftCorner.z + _height / 3f;
        _heightTwoThirds = _bottomLeftCorner.z + _height * 2f / 3f;

        _heightOneSixths = _bottomLeftCorner.z + _height / 6f;
        _heightThreeSixths = _bottomLeftCorner.z + _height * 3f / 6f;
        _heightFiveSixths = _bottomLeftCorner.z + _height * 5f / 6f;

        GameManager.BreedMePlease(_team1, _team2);
    }

    /// <summary>
    /// Assigne le ballon créé puis le positionne ainsi que les joueurs
    /// </summary>
    /// <param name="ball">Le ballon</param>
    public static void Init(Ball ball)
    {
        _instance._ball = ball;

        ball.transform.position = _instance.transform.position;

        _instance.SetTeamPosition();
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
        return transform.TransformPoint(new Vector3(vector.x * _width / 2f, 1f, vector.y * _height / 2f));
    }
}
