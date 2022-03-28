using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Vector3 _startPoint;
    private Vector3 _interpolator;
    private Vector3 _endPoint;

    private float _speed;

    private bool _isFree = true;
    private bool _isMoving = false;

    private Vector3 _lastVelocity;
    private float _bezierTime;

    public Player Shooter { get; private set; }
    public Player Target { get; private set; }

    private Rigidbody _rgdb;
    private Player Parent => transform.parent?.GetComponent<Player>();

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
        if (_isMoving)
        {
            float length = Vector3.Distance(_startPoint, _interpolator) + Vector3.Distance(_interpolator, _endPoint);

            _bezierTime += Time.deltaTime / (length / _speed);

            if (_bezierTime >= 1f)
            {
                StopMoving();
            }
            else
            {
                Vector3 lastPosition = _rgdb.position;
                Vector3 newPosition = ComputeBezierPosition(_startPoint, _interpolator, _endPoint, _bezierTime);

                _rgdb.MovePosition(newPosition);

                _rgdb.velocity = (newPosition - lastPosition) / (Time.deltaTime / (length / _speed));
            }
        }
        else if (!_isFree)
            transform.localPosition = new Vector3(0, -0.9f + 0.5f, 1.5f);

        //transform.localPosition = (Mathf.Sin(Time.time * 10f) + 2f) * Vector3.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!_isFree) Free();
    }

    #region Shoot

    public void Shoot(Vector3 to, float force)
    {
        Shooter = Parent;

        Free();

        ResetMovements();

        _rgdb.AddForce((to - transform.position).normalized * force, ForceMode.Impulse);
    }

    public void Shoot(Vector3 to, Vector3 interpolator, float force)
    {
        Shooter = Parent;

        Free();

        StartMoving();

        _bezierTime = 0f;

        _startPoint = _rgdb.position;
        _interpolator = interpolator;
        _endPoint = to;

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

    public void LobPass(Vector3 direction)
    {
        Free();

        ResetMovements();

        direction = direction * 20f;

        float h = direction.y;
        direction.y = 0;

        float distance = direction.magnitude;
        float a = 45f * Mathf.Deg2Rad;

        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2f * a));
        _rgdb.AddForce(velocity * direction.normalized, ForceMode.Impulse);
    }

    #endregion

    #region Tools

    public void Free()
    {
        _isFree = true;

        Target = null;
        transform.parent = null;

        StopMoving();
    }

    public void Take(Transform parent)
    {
        if (_isFree && parent != Shooter?.transform)
        {
            _isFree = false;

            transform.parent = parent;
            Target = null;

            StopMoving();
            ResetMovements();
        }
    }

    private void StartMoving()
    {
        _isMoving = true;

        _rgdb.useGravity = false;

        ResetMovements();
    }

    private void StopMoving()
    {
        _isMoving = false;

        Shooter = null;

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
}
