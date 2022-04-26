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
    public Team Enemies => Team == Field.Team1 ? Field.Team2 : Field.Team1;

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

    private int lockFrames = 0;
    private Action _waitingAction = null;

    private Transform _lookAt;

    private float _speed;

    private (bool run, float value) _kickOffTimer = (false, 0f);

    public bool IsGoalKeeper { get; private set; }

    #region Debug

    private bool _isRetard => GameManager.EnemiesAreRetard && Team == Field.Team2 && Time.timeSinceLevelLoad < 20f;

    #endregion

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

            player.SetNavDriven(_positions.Dequeue(), _speeds.Dequeue());
            System.Action anim = _animations.Dequeue();
            if (_playWhileMovingBools.Dequeue())
            {
                anim();
                anim = null;
            }
            return (anim, _durations.Dequeue());
        }
    }

    public PlayerActionsQueue ActionsQueue;
    private bool _processQueue = false;

    private float _timer;
    private float _currentTimeLimit;
    private System.Action _animToPerform;

    #endregion

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
        _agent.avoidancePriority = Mathf.RoundToInt(Random.value * 1000f);

        _lookAt = GetComponent<PlayerHeadControl>()._target.transform;

        ResetBoost();
    }

    public bool debug = false;

    private void Update()
    {
        if(_kickOffTimer.run)
            _kickOffTimer.value += Time.deltaTime;
        _lookAt.position = HasBall ? Enemies.transform.position : Field.Ball.transform.position;

        if (HasBall)
            Field.Ball.SetLoading(0f);

        Team.GainItem();

        _rgdb.angularVelocity = Vector3.zero;
        _rgdb.velocity = Vector3.zero;

        if (_processQueue)
            UpdateNavQueue();

        _agent.enabled = IsNavDriven || !IsPiloted || InWall || IsGoalKeeper;

        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = !IsNavDriven && IsPiloted;

        if (IsNavDriven)
        {
            if (Vector3.Distance(transform.position, _agent.destination) <= 0.1f)
            {
                if (!_processQueue)
                {
                    IsNavDriven = false;
                    _agent.speed = 10f;

                    IsWaiting = true;

                    if (Field.ArePlayersAllWaiting())
                        UIManager._instance.DisplayAnnouncement(UIManager.AnnouncementType.ReadySetGo);

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

            if (Field.ArePlayersAllWaiting())
            {
                GameManager.IsGoalScored = false;
                _kickOffTimer.run = true;
                ResetState();
            }
            else
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

        if ((GameManager.DebugOnlyPlayer && (!HasBall && !IsPiloted)) || _isRetard)
            return;

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
            if (action.ActionType == Action.Type.Pass && _kickOffTimer.value > 2f)
            {
                GameManager.FreePlayers();
                GameManager.ChronoStopped = false;
                _kickOffTimer = (false, 0f);
            }
            else
                return;
        }

        if (debug) Debug.Log(name);

        if (lockFrames++ >= 100 && _agent.isOnNavMesh && _agent.remainingDistance >= 0.1f && _agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.velocity == Vector3.zero)
        {
            _agent.enabled = false;
            _waitingAction = action;
            lockFrames = 0;
        }
        else
            MakeAction(action);
            
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
        
        #endregion
    }

    public void ReadQueue()
    {
        if (_processQueue)
            return;
        _processQueue = true;
        (_animToPerform, _currentTimeLimit) = ActionsQueue.GetNext(this);
    }


    private void ChangeMaterialOnElectrocution(bool enabled)
    {
        if (enabled)
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0], MaterialElectricity };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                _electrocutedBody[i].materials = Mats;
            }

        }
        else
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0] };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
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
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                _electrocutedBody[i].materials = Mats;
            }
        }
        else
        {
            for (int i = 0; i < _electrocutedBody.Length; i++)
            {
                Material[] Mats = new Material[] { _electrocutedBody[i].materials[0] };
                //Debug.Log(gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().materials[1].name);
                _electrocutedBody[i].materials = Mats;
            }

        }
    }

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

    private void MakeAction(Action action)
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
                    _agent.Move(direction * 2f * _speed * Time.deltaTime);
                else
                    _rgdb.position += direction * 2f * _speed * Time.deltaTime;
                Run();

                break;

            case Action.Type.Stop:
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                Idle();

                break;

            case Action.Type.MoveTo:
                _agent.enabled = true;
                _agent.SetDestination(action.Position);
                Run();

                break;

            case Action.Type.Shoot:
                if (HasBall)
                {
                    Shoot(action.Force);
                    PlaySound(AudioManager.charaSFXType.Shoot);
                    _animator.SetTrigger("Strike");
                }

                break;

            case Action.Type.Throw:
                PlaySound(AudioManager.charaSFXType.ThrowItem);
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
                    Field.Ball.LobPass(direction, 20f);
                    PlaySound(AudioManager.charaSFXType.Pass);
                    _animator.SetTrigger("Pass");
                }

                break;

            case Action.Type.Pass:
                if (HasBall)
                {
                    DirectPass(direction);
                    PlaySound(AudioManager.charaSFXType.Pass);
                    _animator.SetTrigger("Pass");
                }

                break;

            case Action.Type.Loading:
                Field.Ball.SetLoading(action.Force);
                Idle();

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

        if (player && player.IsDoped)
        {
            Fall((_rgdb.position - player.transform.position).normalized, 6f * player._specs.Weight / 70f, 1.5f, 1f);
        }

        if (player && CanGetBall && !IsDoped && !IsGoalKeeper)
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

    #region Shoot

    private void Shoot(float force)
    {
        force = Mathf.Max(0.2f, force);

        //Debug.Log(_specs.Accuracy * force);
        Transform goal = Enemies.transform;

        //float angle = Vector3.SignedAngle(transform.forward, goal.forward, Vector3.up);
        //angle = -(Mathf.Abs(angle) - 180f);

        float distance = Vector3.Distance(transform.position, goal.position);

        if (distance > Field.Width / 2f)
            MissedShoot(goal);
        else
        {
            if (Random.value > _specs.Accuracy * force)
            {
                Debug.Log("Poteau");
                GoalPostShoot(goal);
            }
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
        float distance = Vector3.Distance(goal.position, _rgdb.position);
        interpolator -= Vector3.Project((endPosition - _rgdb.position) * distance / 45f, goal.right);

        Field.Ball.Shoot(endPosition, interpolator, 33f);

        // Debug
        //Debug.Log($"Distance < 45m ({distance}) - Tir cadr� ({x} ; {y}).");
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
                direction = mate.transform.position - transform.position;
                float distance = direction.magnitude;

                Field.Ball.LobPass(mate);
            }
            else
                Field.Ball.Pass(mate, 33f);

            //Debug.Log("Passe directe vers " + direction.ToString());
        }
        else
            Field.Ball.LobPass(direction, 20f);
    }

    #endregion

    #region FindMate

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
                float score = - angle - distance;
                //Debug.Log(player.name + " " + score);

                if (best.player == null || score > best.score)
                    best = (player, score);
            }
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

        GameObject itemGo = Instantiate(data.Prefab, transform.position + transform.forward * 2f, Quaternion.identity);
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
        ChangeMaterialOnElectrocution(true); 

        if (stunType == StunType.Electrocuted)
        {
            ChangeMaterialOnElectrocution(true);
            PlaySound(AudioManager.charaSFXType.Electrocuted);
        }
        else if (stunType == StunType.Chomped) ;
        else if (stunType == StunType.Frozen) ;

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

    private void ResetState()
    {
        CanGetBall = true;

        ChangeMaterialOnElectrocution(false);

        _agent.enabled = true;

        State = PlayerState.Moving;

        CancelInvoke();
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

    private void PlaySound(AudioManager.charaSFXType sfxType)
    {
        if(Team == Field.Team1) // not AI
        {
            if(this == Team.Players[0])// captain
            {
                AudioManager._instance.PlayCharaSFX(sfxType, AudioManager._instance._playerCaptainAudio);
            }
            else // Mate
            {
                AudioManager._instance.PlayCharaSFX(sfxType, AudioManager._instance._playerMateAudio);
            }
        }   
        else // AI
        {
            if (this == Team.Players[0])// captain
            {
                AudioManager._instance.PlayCharaSFX(sfxType, AudioManager._instance._aiCaptainAudio);
            }
            else // Mate
            {
                AudioManager._instance.PlayCharaSFX(sfxType, AudioManager._instance._aiMateAudio);
            }
        }
    }
}
