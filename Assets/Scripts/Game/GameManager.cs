using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Match _debugMatch;

    [SerializeField]
    private float _debugMatchDuration = 60f;

    private static GameManager _instance;

    private Queue<Match> _matches;
    private Queue<MatchResult> _results;
    private MatchResult _currentResult;

    private void Awake()
    {
        _instance = this;

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

    }

    /// <summary>
    /// Fournit les coéquipiers à chaque équipe, les place, et instancie le ballon
    /// </summary>
    /// <param name="team1">Spermatozoïde n°1</param>
    /// <param name="team2">Spermatozoïde n°2</param>
    /// <returns>RIENG</returns>
    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = _instance._matches.Dequeue();

        _instance._currentResult = new MatchResult();
        _instance._currentResult.Match = match;

        Player[] teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain1.Prefab, team1);
        teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate1.Prefab, team1);
            teammates[i].gameObject.SetActive(i == 0);
        }

        Player goal1 = Player.CreatePlayer(match.GoalKeeper.Prefab, team1, true);
        goal1.gameObject.SetActive(false);

        team1.Init(teammates, goal1);

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.Captain2.Prefab, team2);
        //teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
        {
            teammates[i] = Player.CreatePlayer(match.Mate2.Prefab, team2);
            teammates[i].gameObject.SetActive(i == 0);
        }

        Player goal2 = Player.CreatePlayer(match.GoalKeeper.Prefab, team2, true);
        goal2.gameObject.SetActive(false);

        team2.Init(teammates, goal2);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());
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
