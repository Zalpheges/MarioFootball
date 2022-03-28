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

    #region Debug

    public void SetActive(bool value)
    {
        _debugOnly = !value;
    }

    public bool _debugOnly = false;

    #endregion

    [SerializeField]
    public PlayerBrain IABrain { get; private set; }

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }

    public bool CanGetBall => !IsStunned && State != PlayerState.Headbutting && !HasBall;
    public bool IsStunned => State == PlayerState.Shocked || State == PlayerState.Falling;

    public bool HasBall => Field.Ball.transform.parent == transform;
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;

    public static Player CreatePlayer(GameObject prefab, Team team, bool isGoalKeeper = false)
    {
        Player player = Instantiate(prefab).GetComponent<Player>();
        player.transform.rotation = team.transform.localRotation;

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
        if (_debugOnly)
            return;

        Action action = IsPiloted ? Team.Brain.GetAction() : IABrain.GetAction();

        Vector3 deltaMove = Field.Transform.TransformDirection(action.DeltaMove);

        switch (action.ActionType)
        {
            case Action.Type.Move:
                _rgbd.position += deltaMove * 10f * _specs.Speed * Time.deltaTime;

                break;

            case Action.Type.Shoot:
                if (HasBall)
                    Shoot();

                break;

            case Action.Type.Throw:
                Debug.Log("Throw");

                break;

            case Action.Type.Headbutt:
                Debug.Log("HeadButt");

                break;

            case Action.Type.Tackle:
                Debug.Log("Tackle");

                break;

            case Action.Type.ChangePlayer:
                Debug.Log("ChangePlayer");

                break;

            case Action.Type.Dribble:
                Debug.Log("Drible");

                break;

            case Action.Type.LobPass:
                if (HasBall)
                    LobPass(action.ShootDirection);

                break;

            case Action.Type.Pass:
                if (HasBall)
                    DirectPass(action.ShootDirection);

                break;

            default:
                break;
        }
    }

    #region Shoot

    private void Shoot()
    {
        Transform goal = (Team == Field.Team1 ? Field.Team2 : Field.Team1).transform;

        //float angle = Vector3.SignedAngle(transform.forward, goal.forward, Vector3.up);
        //angle = -(Mathf.Abs(angle) - 180f);

        float distance = Mathf.Abs(goal.position.z - transform.position.z);

        if (distance > Field.Width / 2f)
            MissedShoot(goal);
        else
        {
            if (Random.value > _specs.Accuracy)
                GoalPostShoot(goal);
            else
                ShootOnTarget(goal);
        }
    }

    private void MissedShoot(Transform goal)
    {
        float sign = Mathf.Sign(Random.value - 0.5f);
        float range = Random.Range(Field.GoalWidth / 2f, Field.Height / 2f);

        Field.Ball.Shoot(goal.position + goal.right * sign * range, 33f);

        // Debug
        float distance = Mathf.Abs(goal.position.z - transform.position.z);
        string direction = sign < 0 ? "gauche" : "droite";
        //Debug.Log($"Distance > 45m ({distance}) - Tir non cadr� � {direction} ({range}m).");
    }

    private void GoalPostShoot(Transform goal)
    {
        float sign = Mathf.Sign(Random.value - 0.5f);

        Field.Ball.Shoot(goal.position + goal.right * sign * Field.GoalWidth / 2f, 33f);

        // Debug
        float distance = Mathf.Abs(goal.position.z - transform.position.z);
        string direction = sign < 0 ? "gauche" : "droit";
        //Debug.Log($"Distance < 45m ({distance}) - Tir sur poteau {direction}.");
    }

    private void ShootOnTarget(Transform goal)
    {
        float x = Random.Range(-1, 2);
        float y = Random.Range(-1, 2);

        Vector3 endPosition = goal.position;
        endPosition.x += x * Field.GoalWidth * 0.85f / 2f;
        endPosition.y += y * Field.GoalHeight * 0.85f / 2f;

        Vector3 interpolator = (transform.position + endPosition) / 2f;
        interpolator.x = endPosition.x;

        Field.Ball.Shoot(endPosition, interpolator, 33f);

        // Debug
        float distance = Mathf.Abs(goal.position.z - transform.position.z);
        //Debug.Log($"Distance < 45m ({distance}) - Tir cadr� ({x} ; {y}).");
    }

    #endregion

    #region Pass

    private void DirectPass(Vector2 direction)
    {
        Player mate = FindMateInDirection(new Vector3(direction.x, 0f, direction.y));

        if (mate)
        {
            Field.Ball.Pass(mate, 16f);

            //Debug.Log("Passe directe vers " + direction.ToString());
        }
        else
            LobPass(direction);
    }

    private void LobPass(Vector2 direction)
    {
        Vector3 dir = transform.TransformDirection(new Vector3(direction.x, 0f, direction.y));

        Field.Ball.LobPass(dir);

        //Debug.Log("Passe lobée vers " + direction.ToString());
    }

    #endregion

    private Player FindMateInDirection(Vector3 direction, float angle = 45f, int iterations = 50)
    {
        float current = -angle / 2f;
        float step = angle / iterations;

        for (int i = 0; i < iterations; ++i)
        {
            Vector3 dir = transform.TransformDirection(Quaternion.AngleAxis(current, Vector3.up) * direction);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, gameObject.layer))
            {
                Player player = hit.transform.GetComponent<Player>();

                if (player && player.Team == Team)
                    return player;
            }

            current += step;
        }

        return null;
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
