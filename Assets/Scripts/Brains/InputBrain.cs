using UnityEngine;

public class InputBrain : PlayerBrain
{
    /// <summary>
    /// Calcule le déplacement que la manette applique au joueur 
    /// </summary>
    /// <param name="team">L'équipe du joueur</param>
    /// <returns>Le vecteur de déplacement.</returns>
    public override Vector2 Move(Team team)
    {
        return Vector2.zero;
    }
}
