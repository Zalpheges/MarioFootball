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

    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = instance.matches.Dequeue();

        instance.currentResult = new MatchResult();
        instance.currentResult.match = match;

        Player[] teammates = new Player[4];

        teammates[0] = Player.CreatePlayer(match.captain1.prefab, team1);

        for (int i = 0; i < 3; ++i)
            teammates[i] = Player.CreatePlayer(match.mate1.prefab, team1);

        Player goal = Player.CreatePlayer(match.goalKeeper.prefab, team1, true);

        team1.Init(teammates, goal);

        teammates[0] = Player.CreatePlayer(match.captain2.prefab, team1);

        for (int i = 0; i < 3; ++i)
            teammates[i] = Player.CreatePlayer(match.mate2.prefab, team1);

        goal = Player.CreatePlayer(match.goalKeeper.prefab, team1, true);

        team2.Init(teammates, goal);

        Field.Init(Instantiate(PrefabManager.Ball).GetComponent<Ball>());
    }

    private IEnumerator Match()
    {
        yield return new WaitForSeconds(instance.debugMatchDuration);

        instance.currentResult.duration = instance.debugMatchDuration;

        instance.currentResult.scoreTeam1 = Field.Team2.Score;
        instance.currentResult.scoreTeam2 = Field.Team1.Score;

        instance.results.Enqueue(instance.currentResult);
    }
}
