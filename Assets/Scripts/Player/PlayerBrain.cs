using UnityEngine;

public abstract class PlayerBrain : MonoBehaviour
{
    public Player Player;

    protected Team Allies => Player.Team;
    protected Team Enemies => Allies == Field.Team1 ? Field.Team2 : Field.Team1;

    protected virtual void Awake()
    {
        Player = GetComponent<Player>();
    }

    /// <summary>
    /// Ask the brain for an action the player will perform
    /// </summary>
    /// <returns>The action.</returns>
    public abstract Action GetAction();
}
