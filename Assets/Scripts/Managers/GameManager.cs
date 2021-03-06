using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Team LosingTeam => _instance._currentResult.LosingTeam;

    public static Chrono Chrono => _instance._chrono;

    public static bool ChronoStopped = true;

    public static bool IsGoalScored = false;

    public static bool CanSkip = false;

    public static (bool run, float value) KickOffTimer = (false, 0f);
    public static int Difficulty { get; private set; }
    public static bool IsMatchOver { get; private set; } = false;

    private static GameManager _instance;

    private Queue<Match> _matches;
    private Queue<MatchResult> _results;
    private MatchResult _currentResult;
    private Chrono _chrono;
    private float _timer = 0f;
    private bool _endOfGameUIDone = false;
    private bool _inMatch = false;
    private int _nGoalsToWin;

    #region Awake/Start/Update

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
    }

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        IsMatchOver = false;
    }

    private void Update()
    {
        if (!_inMatch)
            return;

        if (IsMatchOver)
            MatchOver();

        if (CanSkip && !IsMatchOver)
        {
            if ((Keyboard.current?.enterKey.wasPressedThisFrame ?? false)
                || (Gamepad.current?.startButton.wasPressedThisFrame ?? false))
            {
                UIManager.SkipAnnouncement();
                CameraManager.SkipQueue();
                SkipPlayersQueue();
                CanSkip = false;
            }
        }

        if (KickOffTimer.run)
            KickOffTimer.value += Time.deltaTime;

        //Chrono check
        UIManager.SetChrono(_chrono);
        if (!ChronoStopped)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                --_chrono;
                if (_chrono.Finished)
                {
                    MatchOver();
                }
                --_timer;
            }
        }

        static void SkipPlayersQueue()
        {
            foreach (Team team in Field.Teams)
            {
                foreach (Player player in team.Players)
                {
                    player.SkipQueue();
                }
                team.Goalkeeper.SkipQueue();
            }
        }
    }
    #endregion

    #region Managing

    /// <summary>
    /// Gives each team's players, place them, and instantiate the ball
    /// </summary>
    /// <param name="team1">Controlled team</param>
    /// <param name="team2">AI team</param>
    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = _instance._matches.Dequeue();
        Difficulty = match.AIDifficulty;
        _instance._nGoalsToWin = match.NGoalsToWin;

        UIManager.InitHUD(match.Captain1, match.Captain2);

        _instance._currentResult = new MatchResult
        {
            Match = match
        };

        #region Team 1

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

        #endregion

        #region Team 2

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain2.Prefab, team2);
        teammates[0].name = "Captain Team 2";
        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate2.Prefab, team2);
            teammates[i].name = $"Mate {i} Team 2";
        }

        Player goal2 = Player.CreatePlayer(match.GoalKeeper.Prefab, team2, true);
        goal2.name = "Goal Team 2";

        team2.Init(teammates, goal2);

        #endregion

        //Create allPlayers array
        Player[] allPlayers = new Player[team1.Players.Length + team2.Players.Length + 2];
        team1.Players.CopyTo(allPlayers, 0);
        team2.Players.CopyTo(allPlayers, team1.Players.Length);
        allPlayers[team1.Players.Length + team2.Players.Length] = team1.Goalkeeper;
        allPlayers[team1.Players.Length + team2.Players.Length + 1] = team2.Goalkeeper;

        Ball ball = Instantiate(PrefabManager.Ball).GetComponent<Ball>();
        CameraManager.Init(allPlayers.Select(player => player.transform).ToArray(), ball.transform);
        CameraManager.Follow(allPlayers[0].transform);
        Field.Init(ball);


        _instance._chrono = new Chrono(match.GameTime, 0);
        _instance._inMatch = true;
    }

    public static void AddMatch(PlayerSpecs playerCaptain, PlayerSpecs playerMate, PlayerSpecs AICaptain, PlayerSpecs AIMate, PlayerSpecs GoalKeeper, int gameTime, int goalToWin, int AIDifficulty)
    {
        Match newMatch = new Match()
        {
            Captain1 = playerCaptain,
            Captain2 = AICaptain,
            Mate1 = playerMate,
            Mate2 = AIMate,

            GoalKeeper = GoalKeeper,

            GameTime = gameTime,
            NGoalsToWin = goalToWin,
            AIDifficulty = AIDifficulty
        };

        _instance._matches.Enqueue(newMatch);
    }

    #endregion

    #region Gameplay

    private void MatchOver()
    {
        IsMatchOver = true;
        ChronoStopped = true;
        foreach(Team team in Field.Teams)
        {
            foreach (Player player in team.Players)
                player.IsWaiting = true;
            team.Goalkeeper.IsWaiting = true;
        }
        if (!_endOfGameUIDone)
        {
            _endOfGameUIDone = true;
            if (Field.Team2 == LosingTeam)
                UIManager.EndOfGame(UIManager.GameState.Win);
            else if (Field.Team1 == LosingTeam)
                UIManager.EndOfGame(UIManager.GameState.Loose);
            else
                UIManager.EndOfGame(UIManager.GameState.Draw);
        }

        if (((Gamepad.current?.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic) ?? false)
            || (Keyboard.current?.anyKey.wasPressedThisFrame ?? false)) && _endOfGameUIDone)
        {
            _inMatch = false;
            Destroy(gameObject);
            LevelLoader.LoadNextLevel(0);
        }
    }

    public static void OnGoalScored(Team team)
    {
        AudioManager.PlaySFX(AudioManager.SFXType.Goal); //GoalScoredSound
        AudioManager.PlayCrowdSound(AudioManager.CrowdSoundType.Goal);//GoalcrowdSound

        UIManager.DisplayAnnouncement(UIManager.AnnouncementType.Goal);

        IsGoalScored = true;
        Field.Ball.Free();
        ChronoStopped = true;

        HandleScorer();

        if (team == Field.Team1)
        {
            UIManager.SetScore(scoreTeam2: ++_instance._currentResult.ScoreTeam2);
            if (_instance._currentResult.ScoreTeam2 >= _instance._nGoalsToWin)
            {
                _instance.MatchOver();
                return;
            }
        }
        else if (team == Field.Team2)
        {
            UIManager.SetScore(scoreTeam1: ++_instance._currentResult.ScoreTeam1);
            if (_instance._currentResult.ScoreTeam1 >= _instance._nGoalsToWin)
            {
                _instance.MatchOver();
                return;
            }
    }

        RedirectPlayers(team.Players, team.Other.Players);
        if (!Field.Team1.Players[0].IsPiloted)
        {
            Field.Team1.ChangePlayer(Field.Team1.Players[0].transform.position);
        }

        CanSkip = true;

        Field.Ball.transform.position = team.Players[0].transform.position;

        #region Local functions

        void HandleScorer()
        {
            Player scorer = Field.Ball.LastOwner;
            bool ownGoal = team == scorer.Team;
            void anim() => scorer.Idle(celebrate: !ownGoal, shameful: ownGoal);
            scorer.ActionsQueue.AddAction(scorer.transform.position, 10f, anim, 5f, false);
            CameraManager.CamerasQueue.AddCameraFocus(scorer.transform, 5f);
            CameraManager.ReadQueue();
            CameraManager.LookAt(scorer.transform);
        }

        /// <summary>
        /// Command to the players to head to their start positions, or customs ones if given
        /// </summary>
        /// <param name="attackingPlayers">Players who just conceded a goal</param>
        /// <param name="defendingPlayers">Players who just scored a goal</param>
        /// <param name="positions">Positions for the players to head to, starting with the attackers</param>
        void RedirectPlayers(Player[] attackingPlayers, Player[] defendingPlayers, List<Vector3> positions = null)
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
            RedirectGoalkeeper(Field.Team1);
            RedirectGoalkeeper(Field.Team2);

            void RedirectGoalkeeper(Team team)
            {
                Player gk = team.Goalkeeper;
                gk.ActionsQueue.AddAction(Field.GetGoalKeeperPosition(team), 10f, () => gk.Run(), 1f, true);
                gk.ReadQueue();
            }
        }

        #endregion

    }

    public static void FreePlayers()
    {
        foreach (Team team in Field.Teams)
        {
            foreach (Player player in team.Players)
                player.Free();
            team.Goalkeeper.Free();
        }
    }

    #endregion

}
