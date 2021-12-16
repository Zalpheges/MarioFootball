using UnityEngine;

[System.Serializable]
public abstract class PlayerBrain : MonoBehaviour
{
    public abstract Vector2 Move(Team team);
}
