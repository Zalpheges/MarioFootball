public class MatchResult
{
    public Match Match;

    public int ScoreTeam1;
    public int ScoreTeam2;

    public float Duration;

    public Team LosingTeam
    {
        get
        {
            if (ScoreTeam1 == ScoreTeam2)
                return null;
            else if (ScoreTeam1 > ScoreTeam2)
                return Field.Team2;
            else
                return Field.Team1;
        }
    }
}
