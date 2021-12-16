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

    [SerializeField] public PlayerBrain IABrain;

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }

    public bool CanGetBall => !IsStunned && State != PlayerState.Headbutting && !HasBall;
    public bool IsStunned => State == PlayerState.Shocked || State == PlayerState.Falling;

    public bool HasBall { get => ball; }
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;

    bool fdp = false;

    public static Player CreatePlayer(GameObject prefab, Team team, bool isGoalKeeper = false, bool isFDP = false)
    {
        Player player = Instantiate(prefab).GetComponent<Player>();

        Component brain = player.gameObject.AddComponent(isGoalKeeper ? team.GoalBrainType : team.TeamBrainType);

        player.IABrain = (PlayerBrain)player.GetComponent(brain.GetType());

        player.Team = team;

        player.GetComponent<Rigidbody>().isKinematic = true;

        player.fdp = isFDP;

        return player;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rgbd.mass = specs.weight;
        gameObject.name = specs.name;

        if (fdp)
            gameObject.name = "fdp de merde";

        if (fdp)
            Debug.Log(transform.position);

        if (Team == Field.Team1)
            gameObject.name += "team1";
    }

    private void Update()
    {
        Vector3 move = IsPiloted ? Team.Brain.Move(Team) : IABrain.Move(Team);

        transform.position += move;

        if (fdp)
            Debug.Log(transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
