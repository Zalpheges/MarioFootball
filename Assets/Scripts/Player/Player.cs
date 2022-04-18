using System.Collections.Generic;
using UnityEngine.AI;
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
        Stunned,
        Dribbling
    }

    public struct PlayerActionsQueue
    {
        private Queue<Vector3> _positions;
        private Queue<System.Action> _animations;
        private Queue<float> _preActionDelays;

        private void Init()
        {
            _positions = new Queue<Vector3>();
            _animations = new Queue<System.Action>();
            _preActionDelays = new Queue<float>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="anim">de la forme () => animator.SetBool("fdp")</param>
        /// <param name="delay"></param>
        public void AddAction(Vector3 position, System.Action anim, float delay)
        {
            if (_positions == null)
                Init();
            _positions.Enqueue(position);
            _animations.Enqueue(anim);
            _preActionDelays.Enqueue(delay);
        }

        public (System.Action, float) GetNext(NavMeshAgent agent)
        {
            if (_positions.Count < 1)
                return (null, 0f);

            agent.destination = _positions.Dequeue();
            System.Action anim = _animations.Dequeue();
            return (anim, _preActionDelays.Dequeue());
        }
    }

    [SerializeField]
    private PlayerSpecs _specs;

    private Animator _animator;
    private Rigidbody _rgdb;
    private NavMeshAgent _agent;

    public PlayerBrain IABrain { get; private set; }

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }
    public Team Enemies => Team == Field.Team1 ? Field.Team2 : Field.Team1;

    public bool CanGetBall { get; private set; } = true;
    public bool IsStunned => State == PlayerState.Stunned || State == PlayerState.Falling;

    public bool HasBall => Field.Ball.transform.parent == transform;
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;
    public bool IsNavDriven { get; private set; } = false;
    public bool IsWaiting { get; set; } = false;
    public bool ProcessQueue { get; private set; } = false;

    public GameObject[] ElectrocutedBody;

    public Material MaterialElectricity;

    public Material MaterialFreeze;

    private float _dashSpeed;
    private Vector3 _dashEndPoint;

    private Action _waitingAction = null;

    public PlayerActionsQueue ActionsQueue;

    private float _timer;
    private float _currentTimeLimit;
    private System.Action _nextAnimToPerform;

    #region Debug

    private bool _isRetard => GameManager.EnemiesAreRetard && Team == Field.Team2 && Time.timeSinceLevelLoad < 20f;
    public PlayerState state;
    public bool isPiloted;
    public bool hasBall;
    public Vector3 input;
    public bool isWaiting;
    public bool isNavDriven;

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

        ChangeMaterialOnElectrocution(false);
        _agent.avoidancePriority = Mathf.RoundToInt(Random.value * 1000f);
    }

    private void Update()
    {
        Team.GainItem();

        //bool debug = Field.Team1.Players[0] == this;

        state = State;
        isPiloted = IsPiloted;
        hasBall = HasBall;
        isWaiting = IsWaiting;
        isNavDriven = IsNavDriven;


        _rgdb.angularVelocity = Vector3.zero;
        _rgdb.velocity = Vector3.zero;

        if (ProcessQueue)
            UpdateNavQueue();

        _agent.enabled = IsNavDriven || !IsPiloted;

        if (_agent.enabled)
            _agent.isStopped = !IsNavDriven && IsPiloted;

        if (IsNavDriven)
        {
            if (Vector3.Distance(transform.position, _agent.destination) <= 0.1f)
            {
                if (!ProcessQueue)
                {
                    IsNavDriven = false;

                    IsWaiting = true;

                    ResetState();

                    _animator.SetBool("Idle", true);
                    _animator.SetBool("Run", false);

                    transform.rotation = Quaternion.LookRotation(Enemies.transform.position - transform.position, Vector3.up);
                }
                else
                {
                    if(_timer > 0.2f)
                        _nextAnimToPerform();
                }
            }

            return;
        }
        if ((GameManager.DebugOnlyPlayer && (!HasBall && !isPiloted)) || _isRetard)
            return;

        if (State != PlayerState.Moving)
        {
            _rgdb.position = Vector3.MoveTowards(_rgdb.position, _dashEndPoint, _dashSpeed * Time.deltaTime);
            input = Vector3.up;

            return;
        }

        if (IsWaiting && !Field.ArePlayersAllWaiting())
            return;

        Action action;

        if (_waitingAction)
            action = _waitingAction;
        else if (IsPiloted)
            action = Team.Brain.GetAction();
        else
            action = IABrain.GetAction();

        input = action.Direction;

        if (action.DirectionalAction)
        {
            Vector3 direction = ComputeDirection(action);

            Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            _rgdb.rotation = Quaternion.Slerp(_rgdb.rotation, rotation, 50f * Time.deltaTime);

            if (action.WaitForRotation && Quaternion.Angle(_rgdb.rotation, rotation) > 5f)
            {
                _waitingAction = action;

                return;
            }
        }

        _waitingAction = null;

        if (IsWaiting)
        {
            if (action.ActionType == Action.Type.Pass)
                GameManager.FreePlayers();
            else
                return;
        }

        MakeAction(action);
    }

    public void ReadQueue()
    {
        ProcessQueue = IsNavDriven = true;

        (_nextAnimToPerform, _currentTimeLimit) = ActionsQueue.GetNext(_agent);

    }
    private void UpdateNavQueue()
    {
        if ((_timer += Time.deltaTime) > _currentTimeLimit)
        {
            _timer = 0f;
            (_nextAnimToPerform, _currentTimeLimit) = ActionsQueue.GetNext(_agent);
            if (_currentTimeLimit == 0f)
                ProcessQueue = IsNavDriven = false;
        }
    }

    public void ChangeMaterialOnElectrocution(bool enabled)
    {
        if (enabled)
        {
            for (int i = 0; i < ElectrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials[0], MaterialElectricity };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials = Mats;
            }

        }
        else
        {
            for (int i = 0; i < ElectrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials[0] };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials = Mats;
            }

        }
    }

    public void ChangeMaterialOnFreeze(bool enabled)
    {
        if (enabled)
        {
            for (int i = 0; i < ElectrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials[0], MaterialFreeze };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials = Mats;
            }
        }
        else
        {
            for (int i = 0; i < ElectrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials[0] };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                ElectrocutedBody[i].GetComponent<SkinnedMeshRenderer>().materials = Mats;
            }

        }
    }

    private void MakeAction(Action action)
    {
        Vector3 direction = ComputeDirection(action);

        if (action.DirectionalAction && action.WaitForRotation)
            transform.LookAt(_rgdb.position + direction, Vector3.up);

        if (action)
            _animator.SetBool("Idle", false);
        else
        {
            _animator.SetBool("Idle", true);
            _animator.SetBool("Run", false);
        }

        switch (action.ActionType)
        {
            case Action.Type.Move:
                _rgdb.position += direction * 2f * _specs.Speed * Time.deltaTime;
                _animator.SetBool("Run", true);

                break;

            case Action.Type.MoveTo:
                _agent.enabled = true;
                _agent.SetDestination(action.Position);
                _animator.SetBool("Run", true);

                break;

            case Action.Type.Shoot:
                if (HasBall)
                {
                    Shoot(action.Force);
                    _animator.SetTrigger("Strike");
                }

                break;

            case Action.Type.Throw:
                ThrowItem(direction);

                break;

            case Action.Type.Headbutt:
                Headbutt(direction);

                break;

            case Action.Type.Tackle:
                Tackle(direction);

                break;

            case Action.Type.ChangePlayer:
                Team.ChangePlayer(transform.position);

                break;

            case Action.Type.Dribble:
                State = PlayerState.Dribbling;
                Dash(direction, 9f, 1.2f);
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

    private Vector3 ComputeDirection(Action action)
    {
        Vector3 direction = action.Direction != Vector3.zero ? Field.Transform.TransformDirection(action.Direction) : transform.forward;

        if (action.ActionType == Action.Type.Shoot)
        {
            direction = Enemies.transform.position - transform.position;
            direction.y = 0f;
        }

        return direction.normalized;
    }

    public void SetNavDriven(Vector3 destination)
    {
        IsNavDriven = true;

        _agent.enabled = true;
        _agent.destination = destination;
        _agent.speed = 10f;

        State = PlayerState.Dribbling;

        _animator.SetBool("Idle", false);
        _animator.SetBool("Run", true);
    }

    #region Collisions

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall")
            ResetState();
    }

    private void OnTrigger(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (Field.Ball == ball && !HasBall && CanGetBall)
            ball.Take(this);

        if (other.tag == "Wall" && State != PlayerState.Stunned)
            Stun();

        Player player = other.GetComponent<Player>();

        if (player && CanGetBall)
        {
            if (player.State == PlayerState.Tackling)
            {
                Fall((_rgdb.position - player.transform.position).normalized);

                //if (!HasBall) OnHitWithNoBall();
            }
            else if (player.State == PlayerState.Headbutting)
            {
                Fall((_rgdb.position - player.transform.position).normalized);

                //if (!HasBall) OnHitWithNoBall();
            }
        }
    }

    #endregion

    #region Shoot

    private void Shoot(float force)
    {
        force = Mathf.Max(0.2f, force);

        Transform goal = Enemies.transform;

        //float angle = Vector3.SignedAngle(transform.forward, goal.forward, Vector3.up);
        //angle = -(Mathf.Abs(angle) - 180f);

        float distance = Vector3.Distance(transform.position, goal.position);

        if (distance > Field.Width / 2f)
            MissedShoot(goal);
        else
        {
            if (Random.value > _specs.Accuracy * force)
                GoalPostShoot(goal);
            else
                ShootOnTarget(goal, force);
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

    private void ShootOnTarget(Transform goal, float force)
    {
        float x = Random.Range(-1, 2);
        float y = force < 1f / 3f ? -1f : (force < 2f / 3f ? 0f : 1f);

        Vector3 endPosition = goal.position;
        endPosition += goal.right * x * Field.GoalWidth * 0.85f / 2f;
        endPosition += goal.up * y * Field.GoalHeight * 0.85f / 2f;

        Vector3 interpolator = (_rgdb.position + endPosition) / 2f;
        interpolator += Vector3.Project(endPosition - _rgdb.position, goal.right);

        Field.Ball.Shoot(endPosition, interpolator, 33f);

        // Debug
        float distance = Vector3.Distance(goal.position, _rgdb.position);
        //Debug.Log($"Distance < 45m ({distance}) - Tir cadr� ({x} ; {y}).");
    }

    #endregion

    #region Pass

    private void DirectPass(Vector3 direction)
    {
        Player mate = FindMateInRange(direction, 90f);

        if (mate)
        {
            Field.Ball.Pass(mate, 33f);

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

    private Player FindMateInRange(Vector3 direction, float range, bool standOut = false)
    {
        return FindPlayerInRange(Team, direction, range, standOut);
    }

    private Player FindEnemyInRange(Vector3 direction, float range, bool standOut = false)
    {
        return FindPlayerInRange(Enemies, direction, range, standOut);
    }

    private Player FindPlayerInRange(Team team, Vector3 direction, float range, bool standOut)
    {
        (Player player, float angle) best = (null, 0f);

        foreach (Player player in team.Players)
        {
            if (player == this)
                continue;

            float angle = Vector3.Angle(direction, player.transform.position - transform.position);

            if (angle <= range && (!standOut || IsPlayerStandOut(player)))
                if (best.player == null || angle < best.angle)
                    best = (player, angle);
        }

        return best.player;
    }

    private bool IsPlayerStandOut(Player player)
    {
        Vector3 direction = player.transform.position - transform.position;

        Debug.DrawRay(transform.position + Vector3.up, direction, Color.red, 10f);
        if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, Mathf.Infinity, 1 << gameObject.layer))
        {
            Player target = hit.transform.GetComponent<Player>();

            if (target == player)
                return true;
        }

        return false;
    }



    #endregion

    #region ThrowItem

    private void ThrowItem(Vector3 direction)
    {
        ItemData data = Team.GetItem();

        if (!data)
            return;

        GameObject itemGo = Instantiate(data.Prefab, transform.position, Quaternion.identity);
        itemGo.GetComponent<Item>().Init(data, this, direction);
    }

    #endregion

    #region Events
    
    public void OnMissedShoot()

    {

        Team.GainItem();

    }

    public void OnHitWithNoBall()

    {

        Team.GainItem();

    }
    
    #endregion

    #region Special

    private void Headbutt(Vector3 direction)
    {
        State = PlayerState.Headbutting;
        _animator.SetTrigger("Electrocuted");

        Dash(direction, 3f, 0.2f);
    }

    private void Tackle(Vector3 direction)
    {
        State = PlayerState.Tackling;
        _animator.SetTrigger("Tackled");

        Dash(direction, 8f, 1.2f, 0.5f);
    }

    private void Fall(Vector3 direction)
    {
        State = PlayerState.Falling;
        _animator.SetTrigger("isTackled");

        if (HasBall)
            Field.Ball.Free();

        Dash(direction, 4f, 1.5f, 2f);
    }

    private void Stun(float duration = 2f)
    {
        State = PlayerState.Stunned;
        _animator.SetTrigger("Electrocuted");
        ChangeMaterialOnElectrocution(true);

        Dash(Vector3.zero, 0f, duration);

        if (HasBall)
        {
            Field.Ball.Free();

            Vector3 direction = Field.Transform.position - transform.position;
            direction = Vector3.Project(direction, Field.Transform.forward);

            Field.Ball.transform.position = transform.position + direction.normalized;
        }
    }

    private void Dash(Vector3 direction, float distance, float time, float standUpDelay = 0f)
    {
        _waitingAction = null;

        CanGetBall = false;

        _animator.SetBool("Idle", false);
        _animator.SetBool("Run", false);

        _agent.enabled = false;

        if (direction == Vector3.zero)
            _dashSpeed = 0f;
        else
        {
            _dashSpeed = distance / time;
            _dashEndPoint = _rgdb.position + direction * distance;
        }

        Invoke(nameof(ResetState), time + standUpDelay);
    }

    private void ResetState()
    {
        CanGetBall = true;

        ChangeMaterialOnElectrocution(false);

        _agent.enabled = true;

        State = PlayerState.Moving;

        CancelInvoke();
    }

    #endregion
}
