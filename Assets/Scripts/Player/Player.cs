using UnityEngine;
using UnityEngine.AI;

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
    private Rigidbody _rgdb;
    private NavMeshAgent _agent;

    public PlayerBrain IABrain { get; private set; }

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }

    public bool CanGetBall => !IsStunned && State != PlayerState.Headbutting && !HasBall;
    public bool IsStunned => State == PlayerState.Shocked || State == PlayerState.Falling;

    public bool HasBall => Field.Ball.transform.parent == transform;
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;
    public bool IsNavDriven { get; set; } = false;

    private Action _waitingAction = null;

    #region Debug

    public void SetActive(bool value)
    {
        _debugOnly = !value;
    }

    public bool _debugOnly = false;
    private bool _isRetard => GameManager.EnemiesAreRetard && Team == Field.Team2 && Time.timeSinceLevelLoad < 2f;

    #endregion

    #region Constructor

    public static Player CreatePlayer(Player prefab, Team team, bool isGoalKeeper = false)
    {
        Player player = Instantiate(prefab);

        Component brain = player.gameObject.AddComponent(isGoalKeeper ? team.GoalBrainType : team.TeamBrainType);

        player.IABrain = (PlayerBrain)player.GetComponent(brain.GetType());

        player.Team = team;

        return player;
    }
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rgdb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _rgdb.mass = _specs.Weight;
        gameObject.name += " " + _specs.Name;
        _agent.enabled = false;
    }

    private void Update()
    {
        //bool debug = Field.Team1.Players[0] == this;

        _rgdb.angularVelocity = Vector3.zero;
        //_rgdb.velocity = Vector3.zero;

        if (_debugOnly || _isRetard)
            return;

        if (IsPiloted)
            Debug.Log(name);

        if (IsNavDriven && Vector3.Distance(transform.position, _agent.destination) <= 0.1f)
        { 
            IsNavDriven = false;
            _agent.enabled = false;
        }

        Action action;

        if (_waitingAction)
            action = _waitingAction;
        else if (IsNavDriven)
            action = Action.NavMove();
        else if (IsPiloted)
            action = Team.Brain.GetAction();
        else
            action = IABrain.GetAction();

        if (action.DirectionnalAction)
        {
            Vector3 direction = action.Direction.magnitude > 0.1f ? action.Direction : transform.forward;

            direction = Field.Transform.TransformDirection(direction);

            if (action.ActionType == Action.Type.Shoot)
            {
                direction = (Team == Field.Team1 ? Field.Team2 : Field.Team1).transform.position - transform.position;
                direction.y = 0f;
            }

            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            _rgdb.MoveRotation(Quaternion.Slerp(_rgdb.rotation, rotation, 100f * Time.deltaTime));

            //if (debug)  Debug.Log(Quaternion.Angle(_rgdb.rotation, rotation) + " " + action.ActionType);

            if (action.WaitForRotation && Quaternion.Angle(_rgdb.rotation, rotation) > 5f)
            {
                _waitingAction = action;

                return;
            }
        }
        _waitingAction = null;
        MakeAction(action);
    }

    private void MakeAction(Action action)
    {
        
        if (_rgdb.velocity.magnitude<0.2f)
        {
            _animator.SetBool("Run 0", false);
            _animator.SetBool("Idl 0", true);
        }
        else
        {
            _animator.SetBool("Run 0", true);
            _animator.SetBool("Idl 0", false);

        }
        bool isPlayingAnimation = _animator.GetCurrentAnimatorStateInfo(0).IsTag("1");

        if (isPlayingAnimation)
            return;

        Vector3 direction = Field.Transform.TransformDirection(action.Direction);
        /*
        if (action == Action.None)
        {
            _animator.SetBool("Idl 0", true);
            _animator.SetBool("Run 0", false);
        }
        else
        {
            _animator.SetBool("Idl 0", false);
        }*/

        switch (action.ActionType)
        {
            case Action.Type.NavMove:
                break;

            case Action.Type.Move:
                _rgdb.MovePosition(_rgdb.position + direction * 8f * _specs.Speed * Time.deltaTime);
                _animator.SetBool("Run 0",true);

                break;

            case Action.Type.Shoot:
                if (HasBall)
                {
                    Shoot();
                    _animator.SetTrigger("Strike");
                }

                break;

            case Action.Type.Throw:
                Debug.Log("Throw");

                break;

            case Action.Type.Headbutt:
                Debug.Log("HeadButt");
                _animator.SetTrigger("Header");

                break;

            case Action.Type.Tackle:
                Debug.Log("Tackle");
                _animator.SetTrigger("Tackled");

                break;

            case Action.Type.ChangePlayer:
                Debug.Log("ChangePlayer");

                break;

            case Action.Type.Dribble:
                Debug.Log("Dribble");
                _animator.SetTrigger("Spin");

                break;

            case Action.Type.LobPass:
                if (HasBall)
                {
                    LobPass(direction);
                    _animator.SetTrigger("Pass");
                }

                break;

            case Action.Type.Pass:
                if (HasBall)
                {
                    DirectPass(direction);
                    _animator.SetTrigger("Pass");
                }

                break;
                
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball ball = collision.transform.GetComponent<Ball>();

        if (Field.Ball == ball && !HasBall)
            ball.Take(transform);
    }

    #region Shoot

    private void Shoot()
    {
        Transform goal = (Team == Field.Team1 ? Field.Team2 : Field.Team1).transform;

        //float angle = Vector3.SignedAngle(transform.forward, goal.forward, Vector3.up);
        //angle = -(Mathf.Abs(angle) - 180f);

        float distance = Vector3.Distance(transform.position, goal.position);

        if (distance > Field.Width / 2f)
            MissedShoot(goal);
        else
        {
            if (Random.value > _specs.Accuracy + 1000f)
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
        float distance = Vector3.Distance(goal.position, _rgdb.position);
        string direction = sign < 0 ? "gauche" : "droite";
        //Debug.Log($"Distance > 45m ({distance}) - Tir non cadr� � {direction} ({range}m).");
    }

    private void GoalPostShoot(Transform goal)
    {
        float sign = Mathf.Sign(Random.value - 0.5f);

        Field.Ball.Shoot(goal.position + goal.right * sign * Field.GoalWidth / 2f, 33f);

        // Debug
        float distance = Vector3.Distance(goal.position, _rgdb.position);
        string direction = sign < 0 ? "gauche" : "droit";
        //Debug.Log($"Distance < 45m ({distance}) - Tir sur poteau {direction}.");
    }

    private void ShootOnTarget(Transform goal)
    {
        float x = Random.Range(-1, 2);
        float y = Random.Range(-1, 2);

        Vector3 endPosition = goal.position;
        endPosition += goal.right * x * Field.GoalWidth * 0.85f / 2f;
        endPosition += goal.up * y * Field.GoalHeight * 0.85f / 2f;

        Vector3 interpolator = (_rgdb.position + endPosition) / 2f;
        interpolator -= Vector3.Project(endPosition - _rgdb.position, goal.right);

        Field.Ball.Shoot(endPosition, interpolator, 33f);

        // Debug
        float distance = Vector3.Distance(goal.position, _rgdb.position);
        //Debug.Log($"Distance < 45m ({distance}) - Tir cadr� ({x} ; {y}).");
    }

    #endregion

    #region Pass

    private void DirectPass(Vector3 direction)
    {
        Player mate = FindMateInRange(direction);

        if (mate)
        {
            Field.Ball.Pass(mate, 16f);

            //Debug.Log("Passe directe vers " + direction.ToString());
        }
        else
            LobPass(direction);
    }

    private void LobPass(Vector3 direction)
    {
        Field.Ball.LobPass(direction);

        //Debug.Log("Passe lobée vers " + direction.ToString());
    }

    #endregion

    #region FindMate

    private Player FindMateInRange(Vector3 direction, float angle = 180f, int iterations = 100)
    {
        float current = 0;
        float step = angle / iterations / 2f;

        for (int i = 0; i < iterations; ++i)
        {
            Player mate = FindMateInDirection(Quaternion.AngleAxis(current, Vector3.up) * direction);

            mate ??= FindMateInDirection(Quaternion.AngleAxis(-current, Vector3.up) * direction);

            if (mate)
                return mate;

            current += step;
        }

        return null;
    }

    private Player FindMateInDirection(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, Mathf.Infinity, 1 << gameObject.layer))
        {
            Player player = hit.transform.GetComponentInParent<Player>();

            if (player && player.Team == Team)
                return player;
        }

        return null;
    }

    #endregion
}
