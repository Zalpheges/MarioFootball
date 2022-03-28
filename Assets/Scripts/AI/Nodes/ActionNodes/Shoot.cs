using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Shoot : Node
{
    public override (NodeState, Action) Evaluate()
    {
        //Debug.Log("SHOOT!");
        return (NodeState.SUCCESS, Action.None);
    }
}
