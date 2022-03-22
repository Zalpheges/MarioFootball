using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Proximity : Node
{
    private Transform _target;

    public Proximity(Transform target)
    {
        _target = target;
    }

    public override NodeState Evaluate()
    {
        return NodeState.RUNNING;
    }
}
