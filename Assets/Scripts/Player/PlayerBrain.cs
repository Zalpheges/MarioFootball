using UnityEngine;

[System.Serializable]
public abstract class PlayerBrain : MonoBehaviour
{
    /// <summary>
    /// Calcule le déplacement que l'IA doit appliquer au joueur 
    /// </summary>
    /// <param name="team">L'équipe du joueur</param>
    /// <returns>Le vecteur de déplacement.</returns>
    public abstract Vector2 Move(Team team);
}
