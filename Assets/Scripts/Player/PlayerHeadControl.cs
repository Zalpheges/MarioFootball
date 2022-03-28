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

    private GameObject Weight;
    


    void Update()
    {
        //Debug.Log(_headposition.transform.position - _target.transform.position);
        float xposition = _headposition.transform.position.x - _target.transform.position.x;
        //Debug.Log(xposition);
        if (xposition>0.6f  || xposition<-0.6f)
        {
            //_headRig.weight = 1;
            _headRig.weight=Mathf.Lerp(_headRig.weight, 1, Time.deltaTime * 3);
        }
        else
        {
            //_headRig.weight = 0;
            _headRig.weight=Mathf.Lerp(_headRig.weight, 0, Time.deltaTime * 3);

        }





    }
}
