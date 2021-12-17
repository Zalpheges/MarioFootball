using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Match debugMatch;
    [SerializeField] private float debugMatchDuration = 60f;

    private static GameManager instance;
    private Queue<Match> matches;
    private Queue<MatchResult> results;
    private MatchResult currentResult;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        matches = new Queue<Match>();
        matches.Enqueue(debugMatch);
    }

    /// <summary>
    /// Fournit les coéquipiers à chaque équipe, les place, et instancie le ballon
    /// </summary>
    /// <param name="team1">Spermatozoïde n°1</param>
    /// <param name="team2">Spermatozoïde n°2</param>
    /// <returns>RIENG</returns>
    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = instance.matches.Dequeue();

        instance.currentResult = new MatchResult();
        instance.currentResult.match = match;

        Player[] teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.captain1.prefab, team1);
        teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
            teammates[i] = Player.CreatePlayer(match.mate1.prefab, team1);

        Player goal1 = Player.CreatePlayer(match.goalKeeper.prefab, team1, true);

        team1.Init(teammates, goal1);

        teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.captain2.prefab, team2);
        teammates[0].IsPiloted = true;

        for (int i = 1; i < 4; ++i)
            teammates[i] = Player.CreatePlayer(match.mate2.prefab, team2);

        Player goal2 = Player.CreatePlayer(match.goalKeeper.prefab, team2, true);

        team2.Init(teammates, goal2);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());
    }

    private IEnumerator Match()
    {
        yield return new WaitForSeconds(instance.debugMatchDuration);

        instance.currentResult.duration = instance.debugMatchDuration;

        instance.currentResult.scoreTeam1 = Field.Team2.ConcededGoals;
        instance.currentResult.scoreTeam2 = Field.Team1.ConcededGoals;

        instance.results.Enqueue(instance.currentResult);
    }
}
