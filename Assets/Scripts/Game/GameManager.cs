using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Match debugMatch;

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
        
        Player[] firstTeam = new Player[5];
        Player[] secondTeam = new Player[5];

        firstTeam[0] = Instantiate(match.captain1.prefab).GetComponent<Player>();
        secondTeam[0] = Instantiate(match.captain2.prefab).GetComponent<Player>();
        firstTeam[1] = Instantiate(match.goalKeeper.prefab).GetComponent<Player>();
        secondTeam[1] = Instantiate(match.goalKeeper.prefab).GetComponent<Player>();
        for (int i = 2; i < 5; ++i)
        {
            firstTeam[i] = Instantiate(match.mate1.prefab).GetComponent<Player>();
            secondTeam[i] = Instantiate(match.mate2.prefab).GetComponent<Player>();
        }

        team1.Init(firstTeam);
        team2.Init(secondTeam);
    }

    private void Update()
    {

    }
}
