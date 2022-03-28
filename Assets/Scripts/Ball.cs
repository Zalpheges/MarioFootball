using UnityEngine;

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

    public Player Target { get; private set; }

    private Rigidbody _rgdb;

    private void Awake()
    {
        _rgdb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isMoving)
        {
            float length = Vector3.Distance(_startPoint, _interpolator) + Vector3.Distance(_interpolator, _endPoint);

            _bezierTime += Time.deltaTime / (length / _speed);

            if (_bezierTime >= 1f)
            {
                _rgdb.position = _endPoint;

                StopMoving();
            }
            else
            {
                float coeff = 1 - _bezierTime;
                Vector3 newPosition = Mathf.Pow(coeff, 2f) * _startPoint + 2f * _bezierTime * coeff * _interpolator + Mathf.Pow(_bezierTime, 2f) * _endPoint;

                _lastVelocity = newPosition - _rgdb.position;

                _rgdb.position = newPosition;
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
        if (_isFree)
        {
            _isFree = false;

            transform.parent = parent;
            Target = null;

            StopMoving();
        }
    }

    private void StartMoving()
    {
        _isMoving = true;

        SetKinematic();
    }

    private void StopMoving()
    {
        _isMoving = false;

        _rgdb.useGravity = true;

        ResetMovements();

        _rgdb.velocity = _lastVelocity;
    }

    private void SetKinematic()
    {
        _rgdb.useGravity = false;

        ResetMovements();
    }

    private void ResetMovements()
    {
        _rgdb.velocity = Vector3.zero;
        _rgdb.angularVelocity = Vector3.zero;
    }

    #endregion
}
