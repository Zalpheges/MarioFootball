using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private Team team1, team2;

    private void Start()
    {
        GameManager.BreedMePlease(team1, team2);
    }
}
