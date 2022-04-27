using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerHeadControl : MonoBehaviour
{
    [SerializeField] private GameObject _headposition;
    [SerializeField] private GameObject _target;
    [SerializeField] private Rig _headRig;
    [SerializeField] private float _minlookatX = -0.6f;
    [SerializeField] private float _maxlookatX = 0.6f;
    [SerializeField] private float _minlookatY = 200f;
    [SerializeField] private float _maxlookatY = 200f;

    public GameObject Target => _target;

    void Update()
    {
        float xposition = _headposition.transform.position.x - _target.transform.position.x;
        float yposition = _headposition.transform.position.z - _target.transform.position.z;

        if (xposition < _minlookatX || xposition > _maxlookatX)
        {
            if (yposition < _minlookatY || yposition > _maxlookatY)
            {
                _headRig.weight = Mathf.Lerp(_headRig.weight, 1, Time.deltaTime * 3);
            }
            else
            {
                _headRig.weight = Mathf.Lerp(_headRig.weight, 0, Time.deltaTime * 3);
            }

        }
        else
        {
            _headRig.weight = Mathf.Lerp(_headRig.weight, 0, Time.deltaTime * 3);
        }
    }
}
