using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Match _debugMatch;

    [SerializeField]
    private float _debugMatchDuration = 60f;

    [SerializeField]
    private bool _debugOnlyPlayer = false;

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
            Destroy(this);

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
        UIManager.SetChrono(_chrono);
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
        teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate1.Prefab, team1);

            if (_instance._debugOnlyPlayer)
                teammates[i].SetActive(i == 0);
        }

        Player goal1 = Player.CreatePlayer(match.GoalKeeper.Prefab, team1, true);
        goal1.gameObject.SetActive(false);

        team1.Init(teammates, goal1);

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain2.Prefab, team2);
        if (_instance._debugOnlyPlayer)
            teammates[0].gameObject.SetActive(false);

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate2.Prefab, team2);

            if (_instance._debugOnlyPlayer)
                teammates[i].SetActive(false);
        }

        Player goal2 = Player.CreatePlayer(match.GoalKeeper.Prefab, team2, true);
        goal2.gameObject.SetActive(false);

        team2.Init(teammates, goal2);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());

        Player[] allPlayers = new Player[team1.Players.Length + team2.Players.Length];
        team1.Players.CopyTo(allPlayers, 0);
        team2.Players.CopyTo(allPlayers, team1.Players.Length);
        CameraManager.Init(allPlayers.Select(player => player.transform).ToArray(), Field.Ball.transform);
        CameraManager.Follow(allPlayers[0].transform);
    }

    private IEnumerator Match()
    {
        yield return new WaitForSeconds(_instance._debugMatchDuration);

        _instance._currentResult.Duration = _instance._debugMatchDuration;

        _instance._currentResult.ScoreTeam1 = Field.Team2.ConcededGoals;
        _instance._currentResult.ScoreTeam2 = Field.Team1.ConcededGoals;

        _instance._results.Enqueue(_instance._currentResult);
    }
}
