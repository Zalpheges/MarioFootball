using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Moving,
        Tackling,
        Headbutting,
        Shooting,
        Falling,
        Shocked
    }

    [SerializeField] private PlayerSpecs specs;

    private Ball ball;

    private Animator animator;
    private Rigidbody rgbd;

    [SerializeField] private PlayerBrain IABrain;

    public PlayerState State { get; private set; }
    public Team Team { get; set; }

    public bool CanGetBall => !IsStunned && State != PlayerState.Headbutting && !HasBall;
    public bool IsStunned => State == PlayerState.Shocked || State == PlayerState.Falling;

    public bool HasBall { get => ball; }
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rgbd.mass = specs.weight;
        gameObject.name = specs.name;
    }

    private void Update()
    {
        Vector3 move = IsPiloted ? Team.Brain.Move() : IABrain.Move();

        transform.position += move;
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
