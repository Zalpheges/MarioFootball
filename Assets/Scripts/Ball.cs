using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Vector3 _startPoint;
    private Vector3 _interpolator;
    public Vector3 EndPoint { get; private set; }

    private float _speed;

    private bool _isFree = true;
    public bool IsMoving { get; private set; } = false;

    private Vector3 _lastVelocity;
    private float _bezierTime;

    public Player Shooter { get; private set; }
    public Player Target { get; private set; }
    public Player LastOwner { get; private set; }

    private Rigidbody _rgdb;
    private Player _parent;

    [SerializeField]
    private ParticleSystem _circle;

    IEnumerator SlowDown()
    {
        yield return new WaitForSeconds(1f);

        //Time.timeScale = 0.1f;
    }

    private void Awake()
    {
        _rgdb = GetComponent<Rigidbody>();

        StartCoroutine(SlowDown());
    }

    private void Update()
    {
        if (IsMoving)
        {
            float length = Vector3.Distance(_startPoint, _interpolator) + Vector3.Distance(_interpolator, EndPoint);

            _bezierTime += Time.deltaTime / (length / _speed);

            if (_bezierTime >= 1f)
            {
                StopMoving();

                Vector3 newPosition = ComputeBezierPosition(_startPoint, _interpolator, EndPoint, 0.95f);
                Vector3 lastPosition = ComputeBezierPosition(_startPoint, _interpolator, EndPoint, 0.1f);

                _rgdb.velocity = (newPosition - lastPosition).normalized * _speed;
            }
            else
            {
                Vector3 lastPosition = _rgdb.position;
                Vector3 newPosition = ComputeBezierPosition(_startPoint, _interpolator, EndPoint, _bezierTime);

                _rgdb.MovePosition(newPosition);
            }
        }
        else if (!_isFree)
        {
            GameObject parent = gameObject.transform.parent.gameObject;
            Animator _animatorparent = parent.GetComponent<Animator>();

            Vector3 BallRun = new Vector3(0, 0, _animatorparent.GetFloat("BallRun") * 20);
            Vector3 BallSpinV = new Vector3(0, 0, _animatorparent.GetFloat("BallSpinV") * 30);

            transform.localPosition = BallRun + BallSpinV + new Vector3(0.113f, 0.2f, 0.979f);

            //transform.localPosition = new Vector3(0, 0.2f, 1.5f);
        }
    }

    #region Shoot

    public void Shoot(Vector3 to, float force)
    {
        Free();

        ResetMovements();

        _rgdb.AddForce((to - transform.position).normalized * force, ForceMode.Impulse);
    }

    public void Shoot(Vector3 to, Vector3 interpolator, float force)
    {
        Free();

        StartMoving();

        _bezierTime = 0f;

        _startPoint = _rgdb.position;
        _interpolator = interpolator;
        EndPoint = to;

        _speed = force;
    }

    #endregion

    #region Pass

    public void Pass(Player target, float force)
    {
        Free();

        ResetMovements();

        Target = target;

        Vector3 direction = (target.transform.position - transform.position);
        direction.y = 0f;

        _rgdb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    public void LobPass(Player target)
    {
        Free();

        Target = target;

        Vector3 direction = target.transform.position - transform.position;

        float distance = direction.magnitude;

        LobPass(direction.normalized, distance);
    }

    public void LobPass(Vector3 direction, float distance)
    {
        if (!_isFree)
            Free();

        ResetMovements();

        float a = 45f * Mathf.Deg2Rad;

        direction.y = Mathf.Tan(a);

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2f * a));
        _rgdb.AddForce(velocity * direction.normalized, ForceMode.Impulse);
    }

    #endregion

    #region Tools

    public void Free()
    {
        Shooter = _parent;

        _isFree = true;
        _parent = null;

        Invoke(nameof(ResetShooter), 1f);

        Target = null;
        transform.parent = null;

        StopMoving();
    }

    public void ResetShooter()
    {
        Shooter = null;
    }

    public void Take(Player parent)
    {
        Player owner = transform.parent?.GetComponent<Player>();

        if (!_isFree && owner.State == Player.PlayerState.Dribbling)
            return;

        if (parent.transform != Shooter?.transform && (!owner || owner.Team != parent?.Team))
        {
            _parent = LastOwner = parent;
            _isFree = false;

            transform.parent = parent.transform;
            Target = null;

            StopMoving();
            ResetMovements();
        }
    }

    private void StartMoving()
    {
        IsMoving = true;

        _rgdb.useGravity = false;

        ResetMovements();
    }

    private void StopMoving()
    {
        IsMoving = false;

        _rgdb.useGravity = true;
    }

    private void ResetMovements()
    {
        _rgdb.velocity = Vector3.zero;
        _rgdb.angularVelocity = Vector3.zero;
    }

    private Vector3 ComputeBezierPosition(Vector3 start, Vector3 interpolation, Vector3 end, float completion)
    {
        float coeff = 1 - completion;
        return Mathf.Pow(coeff, 2f) * start + 2f * completion * coeff * interpolation + Mathf.Pow(completion, 2f) * end;
    }

    #endregion

    public void SetLoading(float force)
    {
        var main = _circle.main;
        main.startSize = force * 3f;
    }
}
