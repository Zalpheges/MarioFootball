using UnityEngine;

public abstract class PlayerBrain : MonoBehaviour
{
    public Player Player { get; set; }

    protected Team Allies => Player.Team;
    protected Team Enemies => Allies == Field.Team1 ? Field.Team2 : Field.Team1;

    protected virtual void Awake()
    {
        Player = GetComponent<Player>();
    }

    protected bool IsOther(Player player)
    {
        return player != Player;
    }

    /// <summary>
    /// Calcule le d�placement que l'IA doit appliquer au joueur 
    /// </summary>
    /// <param name="team">L'�quipe du joueur</param>
    /// <returns>Le vecteur de d�placement.</returns>
    public abstract Action GetAction();
}
