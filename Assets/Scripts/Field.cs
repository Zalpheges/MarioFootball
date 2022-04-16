using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field _instance;

    [SerializeField]
    private Transform _circleSpin;

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
    private Vector2 _posGoalKeeper;

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

    private float _widthOneThird;
    public static float WidthOneThird => _instance._widthOneThird;

    private float _widthTwoThirds;
    public static float WidthTwoThirds => _instance._widthTwoThirds;

    [SerializeField]
    private Vector2 _goalArea;
    public static Vector2 GoalArea => _instance._goalArea;

    private Ball _ball;
    public static Ball Ball => _instance._ball;

    public static Transform Transform => _instance.transform;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        AudioManager._instance.PlayMusic(AudioManager.MusicType.Match); //MatchMusicPlay

        _bottomLeftCorner = transform.TransformPoint(new Vector3(-_width / 2f, 0f, -_height / 2f));
        _bottomRightCorner = transform.TransformPoint(new Vector3(_width / 2f, 0f, -_height / 2f));
        _topLeftCorner = transform.TransformPoint(new Vector3(-_width / 2f, 0f, _height / 2f));
        _topRightCorner = transform.TransformPoint(new Vector3(_width / 2f, 0f, _height / 2f));

        _heightOneThird = _bottomLeftCorner.z + _height / 3f;
        _heightTwoThirds = _bottomLeftCorner.z + _height * 2f / 3f;

        _heightOneSixths = _bottomLeftCorner.z + _height / 6f;
        _heightThreeSixths = _bottomLeftCorner.z + _height * 3f / 6f;
        _heightFiveSixths = _bottomLeftCorner.z + _height * 5f / 6f;

        _widthOneThird = _topLeftCorner.z + _width / 3f;
        _widthTwoThirds = _topLeftCorner.z + _width * 2 / 3f;

        GameManager.BreedMePlease(_team1, _team2);
    }

    private void Update()
    {
        _circleSpin.gameObject.SetActive(!Ball.transform.parent);

        _circleSpin.position = Ball.transform.position;
        _circleSpin.position -= _circleSpin.position.y * Vector3.up;
    }

    /// <summary>
    /// Assigne le ballon créé puis le positionne ainsi que les joueurs
    /// </summary>
    /// <param name="ball">Le ballon</param>
    public static void Init(Ball ball)
    {
        _instance._ball = ball;

        _instance.SetTeamPosition();

        ball.transform.position = _instance.VectorToPosition(_instance._attackPosCaptain);
    }

    private void SetTeamPosition()
    {
        Team1.Players[0].transform.position = VectorToPosition(_attackPosCaptain);
        Team1.Players[0].SetNavDriven(VectorToPosition(_attackPosCaptain));
        Team1.Players[1].transform.position = VectorToPosition(_attackPosMate1);
        Team1.Players[1].SetNavDriven(VectorToPosition(_attackPosMate1));
        Team1.Players[2].transform.position = VectorToPosition(_attackPosMate2);
        Team1.Players[2].SetNavDriven(VectorToPosition(_attackPosMate2));
        Team1.Players[3].transform.position = VectorToPosition(_attackPosMate3);
        Team1.Players[3].SetNavDriven(VectorToPosition(_attackPosMate3));
        Team1.Goalkeeper.transform.position = GetGoalKeeperPosition(Team1);
        Team1.Goalkeeper.SetNavDriven(GetGoalKeeperPosition(Team1));

        Team2.Players[0].transform.position = VectorToPosition(_defPosCaptain);
        Team2.Players[0].SetNavDriven(VectorToPosition(_defPosCaptain));
        Team2.Players[1].transform.position = VectorToPosition(_defPosMate1);
        Team2.Players[1].SetNavDriven(VectorToPosition(_defPosMate1));
        Team2.Players[2].transform.position = VectorToPosition(_defPosMate2);
        Team2.Players[2].SetNavDriven(VectorToPosition(_defPosMate2));
        Team2.Players[3].transform.position = VectorToPosition(_defPosMate3);
        Team2.Players[3].SetNavDriven(VectorToPosition(_defPosMate3));
        Team2.Goalkeeper.transform.position = GetGoalKeeperPosition(Team2);
        Team2.Goalkeeper.SetNavDriven(GetGoalKeeperPosition(Team2));
    }

    public static List<Vector3> GetStartPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        positions.Add(_instance.VectorToPosition(_instance._attackPosCaptain));
        positions.Add(_instance.VectorToPosition(_instance._attackPosMate1));
        positions.Add(_instance.VectorToPosition(_instance._attackPosMate2));
        positions.Add(_instance.VectorToPosition(_instance._attackPosMate3));

        positions.Add(_instance.VectorToPosition(-_instance._defPosCaptain));
        positions.Add(_instance.VectorToPosition(-_instance._defPosMate1));
        positions.Add(_instance.VectorToPosition(-_instance._defPosMate2));
        positions.Add(_instance.VectorToPosition(-_instance._defPosMate3));

        return positions;
    }
    public static bool ArePlayersAllWaiting()
    {
        return Team1.ArePlayersAllWaiting() && Team2.ArePlayersAllWaiting();
    }

    public static Vector3 GetGoalKeeperPosition(Team team)
    {
        float x = team == Field.Team1 ? -_instance._posGoalKeeper.x : _instance._posGoalKeeper.x;
        return _instance.VectorToPosition(new Vector2(x, _instance._posGoalKeeper.y));
    }

    private Vector3 VectorToPosition(Vector2 vector)
    {
        return transform.TransformPoint(new Vector3(vector.x * _width / 2f, 0f, vector.y * _height / 2f));
    }
}
