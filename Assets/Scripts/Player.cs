using UnityEngine;

public class Player : MonoBehaviour
{
    public enum State
    {
        Moving,
        Tackling,
        Headbutting,
        Shooting,
        Falling,
        Shocked
    }

    private PlayerSpecs playerSpecs;

    public State state { get; private set; }

    public bool CanGetBall => !IsStunned && state != State.Headbutting && !HasBall;
    public bool IsStunned => state == State.Shocked || state == State.Falling;

    public bool IsPiloted { get; private set; }
    public bool HasBall { get; private set; }
    public bool IsDoped { get; private set; }

    public bool CanMove => state == State.Moving;

    private Animator animator;
    private Rigidbody rgbd;

    [SerializeField] private PlayerBrain PilotedBrain;
    [SerializeField] private PlayerBrain IABrain;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 move = IsPiloted ? PilotedBrain.Move() : IABrain.Move();

        transform.position += move;
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
