using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float _debugMatchDuration = 60f;

    [SerializeField]
    private bool _debugOnlyPlayer = false;
    public static bool DebugOnlyPlayer => _instance._debugOnlyPlayer;

    [SerializeField]
    private bool _enemiesAreRetard = false;
    [SerializeField]
    private bool _startWithoutAnim = true;
    public static bool EnemiesAreRetard => _instance._enemiesAreRetard;

    public static bool StartWithoutAnim => _instance._startWithoutAnim;

    public static Team LosingTeam => _instance._currentResult.LosingTeam;

    public static Chrono Chrono => _instance._chrono;

    public static bool ChronoStopped = true;

    public static bool IsGoalScored = false;

    private static GameManager _instance;

    private Queue<Match> _matches;
    private Queue<MatchResult> _results;
    private MatchResult _currentResult;
    private Chrono _chrono;
    private float _timer = 0f;
    private bool _endOfGameUIDone =false;

    #region Debug

    public PlayerSpecs d_Captain1;
    public PlayerSpecs d_Captain2;
    public PlayerSpecs d_Mate1;
    public PlayerSpecs d_Mate2;

    public PlayerSpecs d_GoalKeeper;

    public int d_gameTime;
    public float d_goalToWin;
    public int d_aIDifficulty;

    #endregion

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        _matches = new Queue<Match>();

        if (d_Captain1 == null)
            return;

        Match debugMatch = new Match()
        {
            Captain1 = d_Captain1,
            Captain2 = d_Captain2,
            GoalKeeper = d_GoalKeeper,
            Mate1 = d_Mate1,
            Mate2 = d_Mate2,
            gameTime = d_gameTime,
            goalToWin = d_goalToWin,
            AIDifficulty= d_aIDifficulty
        };

        _matches.Enqueue(debugMatch);
    }

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void Update()
    {
        if (!UIManager._instance)
            return;

        if(_chrono.Finished)
        {
            if(!_endOfGameUIDone)
            {
                _endOfGameUIDone = true;
                if (Field.Team2 == LosingTeam)
                    UIManager.EndOfGame(UIManager.gameState.Win);
                else if ( Field.Team1 == LosingTeam)
                    UIManager.EndOfGame(UIManager.gameState.Loose);
                else
                    UIManager.EndOfGame(UIManager.gameState.Draw);
            }

            if (((Gamepad.current?.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic) ?? false) ||  (Keyboard.current?.anyKey.wasPressedThisFrame ?? false)) && _endOfGameUIDone)
            {
                AudioManager._instance.PlayMusic(AudioManager.MusicType.Menu);
                LevelLoader.LoadNextLevel(0);
            }
        }
        UIManager.SetChrono(_chrono);
        if(!ChronoStopped)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                --_chrono;
                if (_chrono.Finished)
                {
                    ChronoStopped = true;
                    Debug.Log("Match end");
                }
                --_timer;
            }
        }
    }

    /// <summary>
    /// Fournit les co�quipiers � chaque �quipe, les place, et instancie le ballon
    /// </summary>
    /// <param name="team1">Spermatozo�de n�1</param>
    /// <param name="team2">Spermatozo�de n�2</param>
    /// <returns>RIENG</returns>
    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = _instance._matches.Dequeue();

        UIManager._instance.InitHUD(match.Captain1, match.Captain2);

        _instance._currentResult = new MatchResult
        {
            Match = match
        };

        Player[] teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain1.Prefab, team1);
        teammates[0].name = "Captain Team 1";
        teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate1.Prefab, team1);
            teammates[i].name = $"Mate {i} Team 1";
        }

        Player goal1 = Player.CreatePlayer(match.GoalKeeper.Prefab, team1, true);
        goal1.name = "Goal Team 1";

        team1.Init(teammates, goal1);

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain2.Prefab, team2);
        teammates[0].name = "Captain Team 2";

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate2.Prefab, team2);
            teammates[i].name = $"Mate {i} Team 2";
        }

        Player goal2 = Player.CreatePlayer(match.GoalKeeper.Prefab, team2, true);
        goal1.name = "Goal Team 2";

        team2.Init(teammates, goal2);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());

        Player[] allPlayers = new Player[team1.Players.Length + team2.Players.Length + 2];
        team1.Players.CopyTo(allPlayers, 0);
        team2.Players.CopyTo(allPlayers, team1.Players.Length);
        allPlayers[team1.Players.Length + team2.Players.Length] = team1.Goalkeeper;
        allPlayers[team1.Players.Length + team2.Players.Length + 1] = team2.Goalkeeper;
        CameraManager.Init(allPlayers.Select(player => player.transform).ToArray(), Field.Ball.transform);
        CameraManager.Follow(allPlayers[0].transform);

        _instance._chrono = new Chrono(match.gameTime, 0);
    }

    public static void OnGoalScored(Team team)
    {
        AudioManager._instance.PlaySFX(AudioManager.SFXType.Goal); //GoalScoredSound
        AudioManager._instance.PlayCrowdSound(AudioManager.CrowdSoundType.Goal);//GoalcrowdSound

        UIManager._instance.DisplayAnnouncement(UIManager.AnnouncementType.Goal);

        IsGoalScored = true;
        Field.Ball.Free();

        Player scorer = Field.Ball.LastOwner;
        bool ownGoal = team == scorer.Team;
        void anim() => scorer.Idle(celebrate: !ownGoal, shameful: ownGoal);
        scorer.ActionsQueue.AddAction(scorer.transform.position, 10f, anim, 5f, false);
        CameraManager.CamerasQueue.AddCameraControl(scorer.transform, 5f);
        CameraManager.ReadQueue();
        CameraManager.LookAt(scorer.transform);
        if (team == Field.Team1)
        {
            UIManager.SetScore(scoreTeam2: ++_instance._currentResult.ScoreTeam2);
            _instance.RedirectPlayers(Field.Team1.Players, Field.Team2.Players);
            //Field.Ball.transform.position = Field.Team2.Players[0].transform.position;
        }
        else if (team == Field.Team2)
        {
            UIManager.SetScore(scoreTeam1: ++_instance._currentResult.ScoreTeam1);
            _instance.RedirectPlayers(Field.Team2.Players, Field.Team1.Players);
            //Field.Ball.transform.position = Field.Team1.Players[0].transform.position;
        }

        Field.Ball.transform.position = Field.Team1.Players[0].transform.position;
        ChronoStopped = true;
    }

    public static void FreePlayers()
    {
        foreach (Player player in Field.Team1.Players)
            player.Free();
        foreach (Player player in Field.Team2.Players)
            player.Free();

        Field.Team1.Goalkeeper.Free();
        Field.Team2.Goalkeeper.Free();
    }

    public static void AddMatch(PlayerSpecs playerCaptain, PlayerSpecs playerMate, PlayerSpecs AICaptain, PlayerSpecs AIMate, int gameTime, float goalToWin, int AIDifficulty)
    {
        Match newMatch = new Match()
        {
            Captain1 = playerCaptain,
            Captain2 = AICaptain,
            Mate1 = playerMate,
            Mate2 = AIMate,

            GoalKeeper = _instance.d_GoalKeeper,

            gameTime = gameTime,
            goalToWin = goalToWin,
            AIDifficulty = AIDifficulty
        };

        _instance._matches.Enqueue(newMatch);
    }

    /// <summary>
    /// Ordonne aux joueurs de se diriger vers leur position de départ, ou vers des positions customs si l'argument positions est renseigné
    /// </summary>
    /// <param name="attackingPlayers">Joueurs possédant le ballon au moment de l'engagement</param>
    /// <param name="defendingPlayers">Joueurs ne possédant pas le ballon au moment de l'engagement</param>
    /// <param name="positions">Positions vers lesquelles se dirigent les joueurs, en commençant par celles de l'équipe attaquantes</param>
    private void RedirectPlayers(Player[] attackingPlayers, Player[] defendingPlayers, List<Vector3> positions = null)
    {
        positions ??= Field.GetStartPositions();
        int n = attackingPlayers.Length;
        for (int i = 0; i < positions.Count; i++)
        {
            bool attackers = i < n;
            Player player = attackers ? attackingPlayers[i] : defendingPlayers[i - n];
            Vector3 destination = player.Team == Field.Team1 ? positions[i] : -positions[i];
            player.ActionsQueue.AddAction(destination, attackers ? 5f : 10f, () => player.Run(happy: !attackers, sad: attackers), 1f, true);
            player.ReadQueue();
        }
        Player gk1 = Field.Team1.Goalkeeper;
        gk1.ActionsQueue.AddAction(Field.GetGoalKeeperPosition(Field.Team1), 10f, () => gk1.Run(), 1f, true);
        gk1.ReadQueue();
        Player gk2 = Field.Team2.Goalkeeper;
        gk2.ActionsQueue.AddAction(Field.GetGoalKeeperPosition(Field.Team2), 10f, () => gk2.Run(), 1f, true);
        gk2.ReadQueue();
    }

    private IEnumerator Match()
    {
        yield return new WaitForSeconds(_instance._debugMatchDuration);

        _instance._currentResult.Duration = _instance._debugMatchDuration;

        _instance._results.Enqueue(_instance._currentResult);
    }
}
