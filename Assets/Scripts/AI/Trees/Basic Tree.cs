using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class BasicTree : BehaviorTree.Tree
{
    public Transform _ballPosition;
    public Transform _playerPosition;
    public bool _hasBall;

    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new GetBall(this),
            new Shoot(this)
        });

        return root;
    }

    public void UpdateVars(Transform iBallPosition, Transform iPlayerPosition, bool iHasBall)
    {
        _ballPosition = iBallPosition;
        _playerPosition = iPlayerPosition;
        _hasBall = iHasBall;
    }
}

