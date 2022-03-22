using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Shoot : Node
{
    private BasicTree _parentTree;

    public Shoot(BasicTree parentTree) => _parentTree = parentTree;

    public override NodeState Evaluate()
    {
        _parentTree.GetComponent<HolyBrain>().actionToPerform = Action.Shoot(0, Vector3.zero, Vector3.zero, 0) ;
        return NodeState.SUCCESS;
    }
}
