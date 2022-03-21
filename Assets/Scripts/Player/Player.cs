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

    [SerializeField]
    private PlayerSpecs _specs;

    private Animator _animator;
    private Rigidbody _rgbd;

    [SerializeField]
    public PlayerBrain IABrain { get; private set; }

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }

    public bool CanGetBall => !IsStunned && State != PlayerState.Headbutting && !HasBall;
    public bool IsStunned => State == PlayerState.Shocked || State == PlayerState.Falling;

    public bool HasBall { get => Field.Ball.transform.parent != transform; }
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;

    public static Player CreatePlayer(GameObject prefab, Team team, bool isGoalKeeper = false)
    {
        Player player = Instantiate(prefab).GetComponent<Player>();

        Component brain = player.gameObject.AddComponent(isGoalKeeper ? team.GoalBrainType : team.TeamBrainType);

        player.IABrain = (PlayerBrain)player.GetComponent(brain.GetType());

        player.Team = team;

        player.GetComponent<Rigidbody>().isKinematic = true;

        return player;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rgbd = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rgbd.mass = _specs.Weight;
        gameObject.name = _specs.name;

        if (Team == Field.Team1)
            gameObject.name += " team1";
        else
            gameObject.name += " team2";
    }

    private void Update()
    {
        Action action = IsPiloted ? Team.Brain.GetAction() : IABrain.GetAction();

        switch (action.ActionType)
        {
            case Action.Type.Move:
                transform.position += action.DeltaMove * 10f * _specs.Speed * Time.deltaTime;

                break;
            case Action.Type.Shoot:

                break;
            case Action.Type.Throw:

                break;
            case Action.Type.Headbutt:

                break;
            case Action.Type.Tackle:

                break;
            case Action.Type.ChangePlayer:

                break;
            case Action.Type.Dribble:

                break;
            case Action.Type.LobPass:

                break;
            case Action.Type.Pass:

                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
