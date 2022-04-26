using System.Collections;
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

    [SerializeField]
    private Transform[] _spawnPointsTeam1;

    [SerializeField]
    private Transform[] _spawnPointsTeam2;

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
        AudioManager.PlayMusic(AudioManager.MusicType.Match); //MatchMusicPlay
        AudioManager.PlayCrowdSound(AudioManager.CrowdSoundType.Normal); //CrowdSound

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
    /// Assigne le ballon cr�� puis le positionne ainsi que les joueurs
    /// </summary>
    /// <param name="ball">Le ballon</param>
    public static void Init(Ball ball)
    {
        _instance._ball = ball;

        if(GameManager.StartWithoutAnim) _instance.SetTeamPosition();
        else _instance.SpawnPlayers();

        ball.transform.position = _instance.VectorToPosition(_instance._attackPosCaptain);
    }

    private void SetTeamPosition()
    {
        Vector3 offset = new Vector3(0f, 0.0f);

        Team1.Players[0].transform.position = VectorToPosition(_attackPosCaptain) + offset;
        Team1.Players[0].SetNavDriven(VectorToPosition(_attackPosCaptain));
        Team1.Players[1].transform.position = VectorToPosition(_attackPosMate1) + offset;
        Team1.Players[1].SetNavDriven(VectorToPosition(_attackPosMate1));
        Team1.Players[2].transform.position = VectorToPosition(_attackPosMate2) + offset;
        Team1.Players[2].SetNavDriven(VectorToPosition(_attackPosMate2));
        Team1.Players[3].transform.position = VectorToPosition(_attackPosMate3) + offset;
        Team1.Players[3].SetNavDriven(VectorToPosition(_attackPosMate3));
        Team1.Goalkeeper.transform.position = GetGoalKeeperPosition(Team1) + offset;
        Team1.Goalkeeper.SetNavDriven(GetGoalKeeperPosition(Team1));

        Team2.Players[0].transform.position = VectorToPosition(_defPosCaptain) + offset;
        Team2.Players[0].SetNavDriven(VectorToPosition(_defPosCaptain));
        Team2.Players[1].transform.position = VectorToPosition(_defPosMate1) + offset;
        Team2.Players[1].SetNavDriven(VectorToPosition(_defPosMate1));
        Team2.Players[2].transform.position = VectorToPosition(_defPosMate2) + offset;
        Team2.Players[2].SetNavDriven(VectorToPosition(_defPosMate2));
        Team2.Players[3].transform.position = VectorToPosition(_defPosMate3) + offset;
        Team2.Players[3].SetNavDriven(VectorToPosition(_defPosMate3));
        Team2.Goalkeeper.transform.position = GetGoalKeeperPosition(Team2) + offset;
        Team2.Goalkeeper.SetNavDriven(GetGoalKeeperPosition(Team2));
    }

    public static List<Vector3> GetStartPositions()
    {
        List<Vector3> positions = new List<Vector3>
        {
            _instance.VectorToPosition(_instance._attackPosCaptain),
            _instance.VectorToPosition(_instance._attackPosMate1),
            _instance.VectorToPosition(_instance._attackPosMate2),
            _instance.VectorToPosition(_instance._attackPosMate3),

            _instance.VectorToPosition(-_instance._defPosCaptain),
            _instance.VectorToPosition(-_instance._defPosMate1),
            _instance.VectorToPosition(-_instance._defPosMate2),
            _instance.VectorToPosition(-_instance._defPosMate3)
        };

        return positions;
    }
    public static bool ArePlayersAllWaiting()
    {
        return Team1.ArePlayersAllWaiting() && Team2.ArePlayersAllWaiting();
    }

    public static Vector3 GetGoalKeeperPosition(Team team)
    {
        float x = team == Team1 ? -_instance._posGoalKeeper.x : _instance._posGoalKeeper.x;
        return _instance.VectorToPosition(new Vector2(x, _instance._posGoalKeeper.y));
    }

    private void SpawnPlayers()
    {
        List<Vector3> startPositions = GetStartPositions();

        static void RunHappy(Player player) => player.Run(happy: true);
        for (int i = 0; i < Team1.Players.Length; i++)
        {
            Player player = Team1.Players[i];
            player.transform.position = _spawnPointsTeam1[i % _spawnPointsTeam1.Length].position;
            player.ActionsQueue.AddAction(startPositions[i], 4f, () => RunHappy(player), 1f, true);
            player.ReadQueue();
            CameraManager.CamerasQueue.AddCameraControl(player.transform, i == 0 ? 0.1f : 1.1f);
        }
        Player goalkeeper1 = Team1.Goalkeeper;
        goalkeeper1.transform.position = _spawnPointsTeam1[Random.Range(0, _spawnPointsTeam1.Length)].position;
        goalkeeper1.ActionsQueue.AddAction(GetGoalKeeperPosition(Team1), 4f, () => RunHappy(goalkeeper1), 0f, true);
        goalkeeper1.ReadQueue();
        CameraManager.CamerasQueue.AddCameraControl(goalkeeper1.transform, 1.1f);

        for (int i = 0; i < Team2.Players.Length; i++)
        {
            Player player = Team2.Players[i];
            player.transform.position = _spawnPointsTeam2[i % _spawnPointsTeam2.Length].position;
            player.ActionsQueue.AddAction(-startPositions[i + Team1.Players.Length], 4f, () => RunHappy(player), 1f, true);
            player.ReadQueue();
            CameraManager.CamerasQueue.AddCameraControl(player.transform, 1.1f);
        }

        Player goalkeeper2 = Team2.Goalkeeper;
        goalkeeper2.transform.position = _spawnPointsTeam2[Random.Range(0, _spawnPointsTeam2.Length)].position;
        goalkeeper2.ActionsQueue.AddAction(GetGoalKeeperPosition(Team2), 4f, () => RunHappy(goalkeeper2), 1f, true);
        goalkeeper2.ReadQueue();
        CameraManager.CamerasQueue.AddCameraControl(goalkeeper2.transform, 1.1f);

        CameraManager.ReadQueue();
    }

    private Vector3 VectorToPosition(Vector2 vector)
    {
        return transform.TransformPoint(new Vector3(vector.x * _width / 2f, 0f, vector.y * _height / 2f));
    }
}
