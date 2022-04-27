using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field _instance;

    [SerializeField]
    private Transform _circleSpin;

    [SerializeField] private float _width;
    public static float Width => _instance._width;

    [SerializeField] private float _height;
    public static float Height => _instance._height;

    [SerializeField] private float _goalWidth;
    public static float GoalWidth => _instance._goalWidth;

    [SerializeField] private float _goalHeight;
    public static float GoalHeight => _instance._goalHeight;

    [SerializeField]
    private Vector2 _goalArea;
    public static Vector2 GoalArea => _instance._goalArea;

    [SerializeField]
    private Team _team1, _team2;
    public static Team Team1 => _instance._team1;
    public static Team Team2 => _instance._team2;

    public static Team[] Teams => new Team[] { _instance._team1, _instance._team2 };

    [SerializeField] private Vector2 _attackPosCaptain;
    [SerializeField] private Vector2 _attackPosMate1;
    [SerializeField] private Vector2 _attackPosMate2;
    [SerializeField] private Vector2 _attackPosMate3;
    [SerializeField] private Vector2 _posGoalKeeper;
    [SerializeField] private Vector2 _defPosCaptain;
    [SerializeField] private Vector2 _defPosMate1;
    [SerializeField] private Vector2 _defPosMate2;
    [SerializeField] private Vector2 _defPosMate3;

    [SerializeField] private Transform[] _spawnPointsTeam1;
    [SerializeField] private Transform[] _spawnPointsTeam2;

    private Dictionary<Team, Transform[]> _spawnPoints = new Dictionary<Team, Transform[]>();

    private float _heightOneThird;
    public static float HeightOneThird => _instance._heightOneThird;

    private float _heightTwoThirds;
    public static float HeightTwoThirds => _instance._heightTwoThirds;

    private Ball _ball;
    public static Ball Ball => _instance._ball;

    public static Transform Transform => _instance.transform;

    private Vector3 _bottomLeftCorner;

    #region Awake/Start/Update

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        AudioManager.PlayMusic(AudioManager.MusicType.Match);
        AudioManager.PlayCrowdSound(AudioManager.CrowdSoundType.Normal);

        _spawnPoints[Team1] = _spawnPointsTeam1;
        _spawnPoints[Team2] = _spawnPointsTeam2;

        _bottomLeftCorner = transform.TransformPoint(new Vector3(-_width / 2f, 0f, -_height / 2f));

        _heightOneThird = _bottomLeftCorner.z + _height / 3f;
        _heightTwoThirds = _bottomLeftCorner.z + _height * 2f / 3f;

        GameManager.BreedMePlease(_team1, _team2);
    }

    private void Update()
    {
        _circleSpin.gameObject.SetActive(!Ball.transform.parent);

        _circleSpin.position = Ball.transform.position;
        _circleSpin.position -= _circleSpin.position.y * Vector3.up;
    }

    #endregion

    /// <summary>
    /// Assign the given ball, then place it and place players
    /// </summary>
    /// <param name="ball">The instantiated ball</param>
    public static void Init(Ball ball)
    {
        _instance._ball = ball;

        if (GameManager.StartWithoutAnim) _instance.SetTeamPosition();
        else SpawnPlayers();

        ball.transform.position = _instance.VectorToPosition(_instance._attackPosCaptain);

        void SpawnPlayers()
        {
            List<Vector3> startPositions = GetStartPositions();

            foreach(Team team in Teams)
            {
                Vector3 spawnPosition;
                Transform[] spawnPoints = _instance._spawnPoints[team];
                for (int i = 0; i < team.Players.Length; i++)
                {
                    spawnPosition = spawnPoints[i % spawnPoints.Length].position;
                    HandlePlayer(team.Players[i], spawnPosition, team == Team1 ? startPositions[i] : -startPositions[i + Team1.Players.Length]);
                }
                spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                HandlePlayer(team.Goalkeeper, spawnPosition, GetGoalKeeperPosition(team));
            }

            CameraManager.ReadQueue();
            GameManager.CanSkip = true;

            void HandlePlayer(Player player, Vector3 spawnPosition, Vector3 destination)
            {
                player.transform.position = spawnPosition;
                player.ActionsQueue.AddAction(destination, 4f, () => player.Run(happy: true), 1f, true);
                player.ReadQueue();
                CameraManager.CamerasQueue.AddCameraFocus(player.transform, 1.1f);
            }
        }
    }

    public static void SkipPlayersQueue()
    {
        foreach (Team team in Teams)
        {
            foreach (Player player in team.Players)
            {
                player.SkipQueue();
            }
            team.Goalkeeper.SkipQueue();
        }
    }


    #region Debug

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

    #endregion

    #region Utility

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

    private Vector3 VectorToPosition(Vector2 vector)
    {
        return transform.TransformPoint(new Vector3(vector.x * _width / 2f, 0f, vector.y * _height / 2f));
    }

    #endregion

}
