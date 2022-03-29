using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Vector3 _startPoint;
    private Vector3 _interpolator;
    private Vector3 _endPoint;

    private float _speed;

    private bool _isFree = true;
    private bool _isMoving = false;

    private Vector3 _lastPosition;
    private float _bezierTime;

    private bool _trail;
    private Color _startTrailColor;
    private Color _endTrailColor;


    private SphereCollider _collider;
    private Rigidbody _rgdb;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _rgdb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    private void Move()
    {

    }

    private void Update()
    {
        _lastPosition = _rgdb.position;

        if (_isMoving)
        {
            float length = Vector3.Distance(_startPoint, _interpolator) + Vector3.Distance(_interpolator, _endPoint);

            _bezierTime += Time.deltaTime / (length / _speed);

            if (_bezierTime >= 1f)
            {
                Free();

                _rgdb.position = _endPoint;

                _rgdb.velocity = (_rgdb.position - _lastPosition).normalized * _speed;
            }
            else
            {
                float coeff = 1 - _bezierTime;
                _rgdb.position = Mathf.Pow(coeff, 2f) * _startPoint + 2f * _bezierTime * coeff * _interpolator + Mathf.Pow(_bezierTime, 2f) * _endPoint;
            }

        }
        else if (!_isFree)
            transform.localPosition = new Vector3(0, -0.9f + 0.5f, 1.5f);

        //transform.localPosition = (Mathf.Sin(Time.time * 10f) + 2f) * Vector3.forward;
    }

    public void Shoot(Vector3 to, float force)
    {
        transform.parent = null;
        _rgdb.isKinematic = false;

        _isFree = true;

        _rgdb.AddForce((to - transform.position).normalized * force, ForceMode.Impulse);
    }

    public void Shoot(Vector3 to, Vector3 interpolator, float force)
    {
        transform.parent = null;
        _rgdb.isKinematic = true;

        _isFree = true;

        _isMoving = true;

        _bezierTime = 0f;

        _startPoint = _rgdb.position;
        _interpolator = interpolator;
        _endPoint = to;

        _speed = force;
    }

    public void Free()
    {
        transform.parent = null;

        _rgdb.isKinematic = false;

        _isFree = true;
        _isMoving = false;
    }

    public void Take(Transform parent)
    {
        transform.parent = parent;

        _rgdb.isKinematic = true;

        _isFree = false;
    }

    /// <summary>
    /// Dessine une traînée derrière la balle
    /// </summary>
    private void DrawTrail()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isFree)
        {
            Free();

            _rgdb.velocity = (_rgdb.position - _lastPosition) / Time.deltaTime;
        }
    }
}
