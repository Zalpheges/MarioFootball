using System.Collections.Generic;
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
        Stunned,
        Dribbling
    }

    public enum StunType
    {
        Electrocuted,
        Chomped,
        Frozen
    }

    [SerializeField]
    private PlayerSpecs _specs;

    private Rigidbody _rgdb;
    private NavMeshAgent _agent;
    private Animator _animator;
    public PlayerBrain IABrain { get; private set; }

    public PlayerState State { get; private set; }
    public Team Team { get; private set; }
    public Team Enemies => Team.Other;

    public bool CanGetBall { get; private set; } = true;
    public bool IsStunned => State == PlayerState.Stunned || State == PlayerState.Falling;
    public bool InWall { get; private set; } = false;

    public bool HasBall => Field.Ball.transform.parent == transform;
    public bool IsDoped { get; private set; }
    public bool CanMove => State == PlayerState.Moving;

    public bool IsPiloted { get; set; } = false;
    public bool IsNavDriven { get; private set; } = false;
    public bool IsWaiting { get; set; } = false;

    private SkinnedMeshRenderer[] _electrocutedBody;

    public Material MaterialElectricity;

    public Material MaterialFreeze;

    private float _dashSpeed;
    private Vector3 _dashEndPoint;

    private int _lockFrames = 0;
    private Action _waitingAction = null;

    private Transform _lookAt;

    private float _speed;

    public bool IsGoalKeeper { get; private set; }

    private float _cooldown = 0f;

    #region Constructor

    public static Player CreatePlayer(Player prefab, Team team, bool isGoalKeeper = false)
    {
        Player player = Instantiate(prefab);

        Component brain = player.gameObject.AddComponent(isGoalKeeper ? team.GoalBrainType : team.TeamBrainType);

        player.IABrain = (PlayerBrain)player.GetComponent(brain.GetType());

        player.Team = team;

        player.IsGoalKeeper = isGoalKeeper;

        return player;
    }
    #endregion

    #region ActionQueue

    public struct PlayerActionsQueue
    {
        private Queue<Vector3> _positions;
        private Queue<float> _speeds;
        private Queue<System.Action> _animations;
        private Queue<float> _durations;
        private Queue<bool> _playWhileMovingBools;
        public Vector3 LastDestination { get; private set; }

        private void Init()
        {
            _positions = new Queue<Vector3>();
            _speeds = new Queue<float>();
            _animations = new Queue<System.Action>();
            _durations = new Queue<float>();
            _playWhileMovingBools = new Queue<bool>();
        }

        public void AddAction(Vector3 position, float moveSpeed, System.Action anim, float duration, bool playWhileMoving)
        {
            if (_positions == null)
                Init();
            _positions.Enqueue(position);
            _speeds.Enqueue(moveSpeed);
            _animations.Enqueue(anim);
            _durations.Enqueue(duration);
            _playWhileMovingBools.Enqueue(playWhileMoving);
        }

        public (System.Action, float) GetNext(Player player)
        {
            if (_positions.Count < 1)
                return (null, 0f);

            player.SetNavDriven(LastDestination = _positions.Dequeue(), _speeds.Dequeue());
            System.Action anim = _animations.Dequeue();
            if (_playWhileMovingBools.Dequeue())
            {
                anim();
                anim = null;
            }
            return (anim, _durations.Dequeue());
        }

        public (System.Action, float) GetLast(Player player)
        {
            if (_positions.Count < 1)
                return (null, 0f);

            while (_positions.Count > 1)
            {
                _positions.Dequeue();
                _speeds.Dequeue();
                _animations.Dequeue();
                _playWhileMovingBools.Dequeue();
                _durations.Dequeue();
            }

            return GetNext(player);
        }
    }

    public PlayerActionsQueue ActionsQueue;
    private bool _processQueue = false;

    private float _timer;
    private float _currentTimeLimit;
    private System.Action _animToPerform;

    public void SkipQueue()
    {
        (_animToPerform, _currentTimeLimit) = ActionsQueue.GetLast(this);
        transform.position = ActionsQueue.LastDestination;
    }

    public void ReadQueue()
    {
        if (_processQueue)
            return;
        _processQueue = true;
        (_animToPerform, _currentTimeLimit) = ActionsQueue.GetNext(this);
    }

    #endregion

    #region Awake/Start/Update

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rgdb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _electrocutedBody = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        _rgdb.mass = _specs.Weight;
        gameObject.name += " " + _specs.Name;

        ChangeMaterialOnElectrocution(false);
        ChangeMaterialOnFreeze(false);
        _agent.avoidancePriority = Mathf.RoundToInt(Random.value * 1000f);

        _lookAt = GetComponent<PlayerHeadControl>().Target.transform;

        ResetBoost();
    }

    private void Update()
    {
        _lookAt.position = HasBall ? Enemies.transform.position : Field.Ball.transform.position;

        _cooldown += Time.deltaTime;

        if (HasBall)
            Field.Ball.SetLoading(0f);

        _rgdb.angularVelocity = _rgdb.velocity = Vector3.zero;

        if (_processQueue)
            UpdateNavQueue();

        _agent.enabled = IsNavDriven || !IsPiloted || InWall || IsGoalKeeper;

        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = !IsNavDriven && IsPiloted;

        if (IsNavDriven)
        {
            if (Vector3.Distance(transform.position, _agent.destination) <= 0.1f)
                OnDestinationReached();
            return;
        }

        if (IsWaiting)
        {
            Idle();

            if (!IsPiloted)
            {
                Quaternion rotation = Quaternion.LookRotation(Enemies.transform.position - transform.position, Vector3.up);
                _rgdb.rotation = Quaternion.Slerp(_rgdb.rotation, rotation, 50f * Time.deltaTime);
            }

            if (!Field.ArePlayersAllWaiting())
                return;
        }

        Action action;

        if (_waitingAction)
            action = _waitingAction;
        else if (IsPiloted)
            action = Team.Brain.GetAction();
        else
            action = IABrain.GetAction();

        if (State != PlayerState.Moving && action.ActionType != Action.Type.ChangePlayer && !IsWaiting)
        {
            _rgdb.position = Vector3.MoveTowards(_rgdb.position, _dashEndPoint, _dashSpeed * Time.deltaTime);

            return;
        }

        if (IsGoalKeeper)
        {
            transform.LookAt(Enemies.transform.position, Vector3.up);
        }
        else if (action.DirectionalAction)
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
            if (action.ActionType == Action.Type.Pass && GameManager.KickOffTimer.value > 6.4f)//Diplay/audio countdown duration
            {
                GameManager.FreePlayers();
                GameManager.ChronoStopped = false;
                GameManager.KickOffTimer = (false, 0f);
            }
            else
                return;
        }

        if (_lockFrames++ >= 100 && _agent.isOnNavMesh && _agent.remainingDistance >= 0.1f && _agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.velocity == Vector3.zero)
        {
            _agent.enabled = false;
            _waitingAction = action;
            _lockFrames = 0;
        }
        else
            MakeAction();

        #region Local functions

        void UpdateNavQueue()
        {
            if ((_timer += Time.deltaTime) > _currentTimeLimit)
            {
                _timer = 0f;
                (_animToPerform, _currentTimeLimit) = ActionsQueue.GetNext(this);
                if (_currentTimeLimit == 0f)
                    _processQueue = false;
            }
        }

        void OnDestinationReached()
        {
            if (!_processQueue)
            {
                IsNavDriven = false;
                _agent.speed = 10f;

                IsWaiting = true;

                if (Field.ArePlayersAllWaiting())
                {
                    if (!GameManager.KickOffTimer.run)
                    {
                        GameManager.CanSkip = false;
                        GameManager.IsGoalScored = false;
                        UIManager.DisplayAnnouncement(UIManager.AnnouncementType.ReadySetGo);
                        AudioManager.PlaySFX(AudioManager.SFXType.Kickoff);
                        GameManager.KickOffTimer.run = true;
                        ResetState();
                    }
                }

                if (IsGoalKeeper)
                    _agent.agentTypeID = GetAgentTypeIDByName("Goal Keeper");

                transform.rotation = Quaternion.LookRotation(Vector3.Project(transform.position - Team.transform.position, Field.Transform.forward));
            }
            else
            {
                if (_animToPerform != null)
                    _animToPerform();
                else
                    _timer += _currentTimeLimit;//force the next animation to come

            }
        }

        void MakeAction()
        {
            Vector3 direction = ComputeDirection(action);

            if (action.DirectionalAction && action.WaitForRotation && !IsGoalKeeper)
                transform.LookAt(_rgdb.position + direction, Vector3.up);

            if (!action)
                Idle();

            switch (action.ActionType)
            {
                case Action.Type.Move:
                    if (_agent.enabled)
                        _agent.Move(_speed * 2f * Time.deltaTime * direction);
                    else
                        _rgdb.position += _speed * 2f * Time.deltaTime * direction;
                    Run();

                    break;

                case Action.Type.Stop:
                    _agent.isStopped = true;
                    _agent.velocity = Vector3.zero;
                    Idle();

                    break;

                case Action.Type.MoveTo:
                    _agent.enabled = true;
                    _agent.speed = (IsGoalKeeper && Field.Ball.IsMoving) ? 30f : 10f;
                    _agent.SetDestination(action.Position);
                    Run();

                    break;

                case Action.Type.Shoot:
                    if (HasBall)
                    {
                        Shoot(action.Force);
                        PlaySound(AudioManager.CharaSFXType.Shoot);
                        _animator.SetTrigger("Strike");
                    }

                    break;

                case Action.Type.Throw:
                    if (_cooldown > 1.5f)
                    {
                        ThrowItem(direction);
                        PlaySound(AudioManager.CharaSFXType.ThrowItem);
                        _cooldown = 0f;
                    }

                    break;

                case Action.Type.Headbutt:
                    if (_cooldown > 1.5f)
                    {
                        Headbutt(direction);
                        _cooldown = 0f;
                    }

                    break;

                case Action.Type.Tackle:
                    if (_cooldown > 1.5f)
                    {
                        Tackle(direction);
                        _cooldown = 0f;
                    }

                    break;

                case Action.Type.ChangePlayer:
                    Team.ChangePlayer(transform.position);

                    break;

                case Action.Type.Dribble:
                    if (_cooldown > 1.5f)
                    {
                        State = PlayerState.Dribbling;
                        Dash(direction, 9f, 1.2f);
                        _animator.SetTrigger("Spin");
                        _cooldown = 0f;
                    }

                    break;

                case Action.Type.LobPass:
                    if (HasBall)
                    {
                        Field.Ball.LobPass(direction, 20f);
                        PlaySound(AudioManager.CharaSFXType.Pass);
                        _animator.SetTrigger("Pass");
                    }

                    break;

                case Action.Type.Pass:
                    if (HasBall)
                    {
                        DirectPass(direction);
                        PlaySound(AudioManager.CharaSFXType.Pass);
                        _animator.SetTrigger("Pass");
                    }

                    break;

                case Action.Type.Loading:
                    Field.Ball.SetLoading(action.Force);
                    _animator.SetTrigger("Strike");
                    Idle();

                    break;
            }
        }
        #endregion
    }

    #endregion

    #region Movement Setup

    public void SetNavDriven(Vector3 destination, float speed = 10f)
    {
        IsNavDriven = true;
        _agent.agentTypeID = GetAgentTypeIDByName("Default Player");

        _agent.enabled = true;
        _agent.destination = destination;
        _agent.speed = speed;

        State = PlayerState.Dribbling;

        Run();
    }

    public void Free()
    {
        State = PlayerState.Moving;
        IsWaiting = false;
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
        if (other.CompareTag("Wall"))
        {
            ResetState();
            InWall = false;
        }
    }

    private void OnTrigger(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (Field.Ball == ball && !HasBall && CanGetBall && (!ball.LastOwner || !ball.LastOwner.IsWaiting))
            ball.Take(this);

        if (other.CompareTag("Wall"))
        {
            if (State == PlayerState.Falling && !InWall)
                Stun(StunType.Electrocuted);

            InWall = true;
        }

        Player player = other.GetComponent<Player>();
        if (!player)
            return;

        if (player.IsDoped)
        {
            Fall((_rgdb.position - player.transform.position).normalized, 6f * player._specs.Weight / 70f, 1.5f, 1f);
        }

        if (CanGetBall && !IsDoped && !IsGoalKeeper)
        {
            if (player.State == PlayerState.Tackling)
            {
                if (!HasBall) OnHitWithNoBall();

                Fall(Vector3.zero, 0f, 1.5f, 2f * player._specs.Weight / 70f);
            }
            else if (player.State == PlayerState.Headbutting)
            {
                if (!HasBall) OnHitWithNoBall();

                Fall((_rgdb.position - player.transform.position).normalized, 6f * player._specs.Weight / 70f, 1.5f, 1f);
            }
        }
    }

    #endregion

    #region FindPlayer

    public Player FindMateInRange(Vector3 direction, float range, bool standOut = false, bool includeGoalKeeper = true)
    {
        return FindPlayerInRange(Team, direction, range, standOut, includeGoalKeeper);
    }

    public Player FindEnemyInRange(Vector3 direction, float range, bool standOut = false, bool includeGoalKeeper = true)
    {
        return FindPlayerInRange(Enemies, direction, range, standOut, includeGoalKeeper);
    }

    private Player FindPlayerInRange(Team team, Vector3 direction, float range, bool standOut, bool includeGoalKeeper)
    {
        (Player player, float score) best = (null, -Mathf.Infinity);

        List<Player> players = new List<Player>(team.Players);

        if (includeGoalKeeper)
            players.Add(team.Goalkeeper);

        foreach (Player player in players)
        {
            if (player == this)
                continue;

            float angle = Vector3.Angle(direction, player.transform.position - transform.position);
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (angle <= range && (!standOut || IsPlayerStandOut(player)))
            {
                float score = -angle - distance;

                if (best.player == null || score > best.score)
                    best = (player, score);
            }
        }

        return best.player;

        bool IsPlayerStandOut(Player player)
        {
            Vector3 direction = player.transform.position - transform.position;

            if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, Mathf.Infinity, 1 << gameObject.layer))
            {
                Player target = hit.transform.GetComponent<Player>();

                if (target == player)
                    return true;
            }

            return false;
        }
    }

    #endregion

    #region Actions

    #region Shoot

    private void Shoot(float force)
    {
        force = Mathf.Max(0.2f, Random.value);

        Transform goal = Enemies.transform;

        float distance = Vector3.Distance(transform.position, goal.position);

        if (distance > Field.Width / 2f)
            MissedShoot();
        else
        {
            if (Random.value > _specs.Accuracy * force)
            {
                GoalPostShoot();
            }
            else
                ShootOnTarget();
        }

        #region Local functions

        void MissedShoot()
        {
            float sign = Mathf.Sign(Random.value - 0.5f);
            float range = Random.Range(Field.GoalWidth / 2f, Field.Height / 2f);

            Field.Ball.Shoot(goal.position + range * sign * goal.right, 33f);
        }

        void GoalPostShoot()
        {
            float sign = Mathf.Sign(Random.value - 0.5f);

            Field.Ball.Shoot(goal.position + Field.GoalWidth * sign * goal.right / 2f, 33f);
        }

        void ShootOnTarget()
        {
            float x = Random.Range(-1, 2);
            float y = force < 1 / 3f ? -1f : (force < 2 / 3f ? 0f : 1f);

            Vector3 endPosition = goal.position;
            endPosition += 0.85f * Field.GoalWidth * x * goal.right / 2f;
            endPosition += 0.85f * Field.GoalHeight * y * goal.up / 2f;

            Vector3 interpolator = (_rgdb.position + endPosition) / 2f;
            float distance = Vector3.Distance(goal.position, _rgdb.position);
            interpolator -= Vector3.Project((endPosition - _rgdb.position) * distance / 45f, goal.right);

            Field.Ball.Shoot(endPosition, interpolator, 33f);
        }

        #endregion
    }


    #endregion

    #region Pass

    private void DirectPass(Vector3 direction)
    {
        Player mate = FindMateInRange(direction, IsGoalKeeper ? 360f : 90f);

        if (mate)
        {
            if (IsGoalKeeper)
            {
                Field.Ball.LobPass(mate);
            }
            else
                Field.Ball.Pass(mate, 33f);
        }
        else
            Field.Ball.LobPass(direction, 20f);
    }

    #endregion

    #region ThrowItem

    private void ThrowItem(Vector3 direction)
    {
        ItemData data = Team.GetItem();

        if (!data)
            return;

        GameObject itemGo = Instantiate(data.Prefab, transform.position + transform.forward * 2f, Quaternion.identity);
        itemGo.GetComponent<Item>().Init(data, this, direction);
    }

    #endregion

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

    #endregion

    #region Constraints

    public void Fall(Vector3 direction, float distance = 4f, float time = 1.5f, float standUpDelay = 2f)
    {
        State = PlayerState.Falling;
        _animator.SetTrigger("isTackled");

        if (HasBall)
            Field.Ball.Free();

        Dash(direction, distance, time, standUpDelay);
    }

    public void Stun(StunType stunType, float duration = 2f)
    {
        State = PlayerState.Stunned;
        _animator.SetTrigger("Electrocuted");

        if (stunType == StunType.Electrocuted)
        {
            ChangeMaterialOnElectrocution(true);
            PlaySound(AudioManager.CharaSFXType.Electrocuted);
        }
        else if (stunType == StunType.Chomped) ;
        else if (stunType == StunType.Frozen)
            ChangeMaterialOnFreeze(true);

        Dash(Vector3.zero, 0f, duration);

        if (HasBall)
        {
            Field.Ball.Free();

            Vector3 direction = Field.Transform.position - transform.position;
            direction = Vector3.Project(direction, Field.Transform.forward);

            Field.Ball.transform.position = transform.position + direction.normalized;
        }
    }

    public void Dash(Vector3 direction, float distance, float time, float standUpDelay = 0f)
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

    #endregion

    #region Boost

    public void StartBoost(float speed = 1.2f, bool invicible = false)
    {
        _speed *= speed;
        IsDoped = invicible;
    }

    public void ResetBoost()
    {
        _speed = _specs.Speed;
        IsDoped = false;
    }

    #endregion

    #region Anim & Sounds

    public void Run(bool happy = false, bool sad = false)
    {
        _animator.SetBool("Run", true);
        _animator.SetBool("Idle", false);

        _animator.SetBool("HappyWalk", happy);
        _animator.SetBool("SadWalk", sad);

        _animator.SetBool("Celebrate", false);
        _animator.SetBool("Shameful", false);
    }
    public void Idle(bool celebrate = false, bool shameful = false)
    {
        _animator.SetBool("Idle", true);
        _animator.SetBool("Run", false);

        _animator.SetBool("Celebrate", celebrate);
        _animator.SetBool("Shameful", shameful);

        _animator.SetBool("HappyWalk", false);
        _animator.SetBool("SadWalk", false);
    }
    private void PlaySound(AudioManager.CharaSFXType sfxType)
    {
        if (Team == Field.Team1) // not AI
        {
            if (this == Team.Players[0])// captain
            {
                AudioManager.PlayCharaSFX(sfxType, AudioManager.PlayerCaptainAudio);
            }
            else // Mate
            {
                AudioManager.PlayCharaSFX(sfxType, AudioManager.PlayerMateAudio);
            }
        }
        else // AI
        {
            if (this == Team.Players[0])// captain
            {
                AudioManager.PlayCharaSFX(sfxType, AudioManager.AiCaptainAudio);
            }
            else // Mate
            {
                AudioManager.PlayCharaSFX(sfxType, AudioManager.AiMateAudio);
            }
        }
    }

    #endregion

    #region Change Material

    private void ChangeMaterialOnElectrocution(bool enabled)
    {
        if (enabled)
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0], MaterialElectricity };
                _electrocutedBody[i].materials = Mats;
            }

        }
        else
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0] };
                _electrocutedBody[i].materials = Mats;
            }

        }
    }

    private void ChangeMaterialOnFreeze(bool enabled)
    {
        if (enabled)
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0], MaterialFreeze };
                _electrocutedBody[i].materials = Mats;
            }
        }
        else
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0] };
                _electrocutedBody[i].materials = Mats;
            }
        }
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

    #region Utility

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

    private int GetAgentTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }

    #endregion
}
