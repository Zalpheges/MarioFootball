using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private Queue<Match> matches;
    private Queue<MatchResult> results;
    private MatchResult _currentResult;

    private void Awake()
    {
        instance = this;
    }

    public static void BreedMePlease(Team team1, Team team2)
    {
        Match match = instance.matches.Dequeue();
        Player[] firstTeam = new Player[5];
        Player[] secondTeam = new Player[5];
        for (int i = 0; i < 3; ++i)
        {

        }

    }

    private void Update()
    {

    }
}
