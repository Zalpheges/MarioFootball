using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Match _debugMatch;

    [SerializeField]
    private float _debugMatchDuration = 60f;

    [SerializeField]
    private bool _debugOnlyPlayer = false;

    [SerializeField]
    private bool _enemiesAreRetard = false;
    public static bool EnemiesAreRetard => _instance._enemiesAreRetard;

    private static GameManager _instance;

    private Queue<Match> _matches;
    private Queue<MatchResult> _results;
    private MatchResult _currentResult;
    private Chrono _chrono;
    private float _timer = 0f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);

        _matches = new Queue<Match>();
        _matches.Enqueue(_debugMatch);
    }

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        _chrono = new Chrono(5, 0);
    }

    private void Update()
    {
        //UIManager.SetChrono(_chrono);
        _timer += Time.deltaTime;
        if (_timer >= 1f)
        {
            --_chrono;
            if(_chrono.Finished)
                Debug.Log("Match end");
            --_timer;
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

            if (_instance._debugOnlyPlayer)
                teammates[i].SetActive(false);
        }

        Player goal1 = Player.CreatePlayer(match.GoalKeeper.Prefab, team1, true);
        goal1.name = "Goal Team 1";
        goal1.gameObject.SetActive(false);

        team1.Init(teammates, goal1);

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain2.Prefab, team2);
        teammates[0].name = "Captain Team 2";

        if (_instance._debugOnlyPlayer)
            teammates[0].gameObject.SetActive(false);

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate2.Prefab, team2);
            teammates[i].name = $"Mate {i} Team 2";

            if (_instance._debugOnlyPlayer)
                teammates[i].SetActive(false);
        }

        Player goal2 = Player.CreatePlayer(match.GoalKeeper.Prefab, team2, true);
        goal1.name = "Goal Team 2";
        goal2.gameObject.SetActive(false);

        team2.Init(teammates, goal2);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());

        Player[] allPlayers = new Player[team1.Players.Length + team2.Players.Length];
        team1.Players.CopyTo(allPlayers, 0);
        team2.Players.CopyTo(allPlayers, team1.Players.Length);
        CameraManager.Init(allPlayers.Select(player => player.transform).ToArray(), Field.Ball.transform);
        CameraManager.Follow(allPlayers[0].transform);
    }

    public static void GoalScored(Team team)
    {
        Field.Ball.Free();
        if (team == Field.Team1)
        {
            UIManager.SetScore(scoreTeam2: ++_instance._currentResult.ScoreTeam2);
            _instance.RedirectPlayers(Field.Team1.Players, Field.Team2.Players);
        }
        else if (team == Field.Team2)
        {
            UIManager.SetScore(scoreTeam1: ++_instance._currentResult.ScoreTeam1);
            _instance.RedirectPlayers(Field.Team2.Players, Field.Team1.Players);
        }
        else
            Debug.Log("This team does not exist");

        Field.Ball.transform.position = Field.Team1.Players[0].transform.position;
    }

    /// <summary>
    /// Ordonne aux joueurs de se diriger vers leur position de départ, ou vers des positions customs si l'argument positions est renseigné
    /// </summary>
    /// <param name="attackingPlayers">Joueurs possédant le ballon au moment de l'engagement</param>
    /// <param name="defendingPlayers">Joueurs ne possédant pas le ballon au moment de l'engagement</param>
    /// <param name="positions">Positions vers lesquelles se dirigent les joueurs, avec en premier celles de l'équipe attaquantes</param>
    private void RedirectPlayers(Player[] attackingPlayers, Player[] defendingPlayers, List<Vector3> positions = null)
    {
        positions ??= Field.GetStartPositions();
        int n = attackingPlayers.Length;
        for (int i = 0; i < positions.Count; i++)
        {
            Player player = i < n ? attackingPlayers[i] : defendingPlayers[i - n];
            player.IsNavDriven = true;
            NavMeshAgent agent = player.GetComponent<NavMeshAgent>();

            agent.enabled = true;
            agent.destination = positions[i];
            agent.speed = 10f;
        }
    }
    private IEnumerator Match()
    {
        yield return new WaitForSeconds(_instance._debugMatchDuration);

        _instance._currentResult.Duration = _instance._debugMatchDuration;

        _instance._results.Enqueue(_instance._currentResult);
    }
}
