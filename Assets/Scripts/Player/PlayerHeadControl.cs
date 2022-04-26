using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerHeadControl : MonoBehaviour
{
    [SerializeField] 
    public GameObject _headposition;
    public GameObject _target;
    public Rig _headRig;
    public float minlookatX = -0.6f;
    public float maxlookatX = 0.6f;
    public float minlookatY = 200f;
    public float maxlookatY = 200f;

    private GameObject Weight;
    
    void Update()
    {
        
        float xposition = _headposition.transform.position.x - _target.transform.position.x;
        float yposition = _headposition.transform.position.z - _target.transform.position.z;
        //Debug.Log(xposition +  " "+ yposition);
        if (xposition< minlookatX || xposition>maxlookatX)
        {
            if (yposition < minlookatY || yposition > maxlookatY)
            {
                //_headRig.weight = 1;
                _headRig.weight = Mathf.Lerp(_headRig.weight, 1, Time.deltaTime * 3);
            }
            else
            {
                //_headRig.weight = 0;
                _headRig.weight = Mathf.Lerp(_headRig.weight, 0, Time.deltaTime * 3);
            }
            
        }
        else
        {
            //_headRig.weight = 0;
            _headRig.weight=Mathf.Lerp(_headRig.weight, 0, Time.deltaTime * 3);
        }
    }
}
