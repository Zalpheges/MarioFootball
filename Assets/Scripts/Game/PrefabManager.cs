    using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    [SerializeField] private GameObject ball;
    public static GameObject Ball => instance.ball;

    private void Awake()
    {
        instance = this;
    }
}
