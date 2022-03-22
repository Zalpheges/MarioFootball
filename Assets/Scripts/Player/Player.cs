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

    public bool HasBall { get => Field.Ball.transform.parent == transform; }
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
                if (HasBall)
                    Shoot(); 

                break;

            default:
                break;
        }
    }

    private void Shoot()
    {
        Transform goal = (Team == Field.Team1 ? Field.Team2 : Field.Team1).transform;

        float angle = Vector3.SignedAngle(transform.forward, goal.forward, Vector3.up);
        angle = -(Mathf.Abs(angle) - 180f);

        float distance = Mathf.Abs(goal.position.z - transform.position.z);

        if (distance > Field.Width / 2f)
        {
            float sign = Mathf.Sign(Random.value - 0.5f);
            float range = Random.Range(Field.GoalWidth / 2f, Field.Height / 2f);

            Field.Ball.Shoot(goal.position + goal.right * sign * range, 33f);

            string direction = sign < 0 ? "gauche" : "droite";
            Debug.Log($"Distance > 45m ({distance}) - Tir non cadré à {direction} ({range}m).");
        }
        else
        {
            if (Random.value > _specs.Accuracy + 1000f)
            {
                float sign = Mathf.Sign(Random.value - 0.5f);

                Field.Ball.Shoot(goal.position + goal.right * sign * Field.GoalWidth / 2f, 33f);

                string direction = sign < 0 ? "gauche" : "droit";
                Debug.Log($"Distance < 45m ({distance}) - Tir sur poteau {direction}.");
            }
            else
            {
                float x = Random.Range(-1, 2);
                float y = Random.Range(-1, 2);

                Vector3 endPosition = goal.position;
                endPosition.x += x * Field.GoalWidth * 0.85f / 2f;
                endPosition.y += y * Field.GoalHeight * 0.85f / 2f;

                Vector3 interpolator = (transform.position + endPosition) / 2f;
                interpolator.x = endPosition.x;

                Field.Ball.Shoot(endPosition, interpolator, 33f);

                Debug.Log($"Distance < 45m ({distance}) - Tir cadré ({x} ; {y}).");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball ball = collision.transform.GetComponent<Ball>();

        if (Field.Ball == ball)
        {
            ball.Take(transform);
        }
    }
}
