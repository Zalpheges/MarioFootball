using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class GetBall : Node
{
    private BasicTree _parentTree;

    public GetBall(BasicTree parentTree) => _parentTree = parentTree;

    public override NodeState Evaluate()
    {
        if (_parentTree._hasBall)
            return NodeState.SUCCESS;
        
        Vector3 Displacement = _parentTree._ballPosition.position - _parentTree._playerPosition.position;
        Displacement.Normalize();
        _parentTree.GetComponent<HolyBrain>().actionToPerform = Action.Move(Displacement);
        Debug.Log(Displacement);

        return NodeState.FAILURE;
    }
}
