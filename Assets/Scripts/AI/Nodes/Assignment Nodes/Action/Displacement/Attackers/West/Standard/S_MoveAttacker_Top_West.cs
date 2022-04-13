using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_MoveAttacker_Top_West : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        float positionX = _root.ballHolder.transform.position.x;
        float positionZ = _root.ballHolder.transform.position.z;

        if (_root.currentPlayerType == PlayerType.Attacker_Top)
        {
            positionX += _root.Attacker_Offset_Standard_Side_Forward.x;
            positionZ += -_root.Attacker_Offset_Standard_Side_Forward.y;
        }
        else if (_root.currentPlayerType == PlayerType.Attacker_Bot)
        {
            positionX += _root.Attacker_Offset_Standard_Side_Sideward.x;
            positionZ += -_root.Attacker_Offset_Standard_Side_Sideward.y;
        }

        _root.Position = new Vector3(positionX, 0, positionZ);
        _root.actionToPerform = ActionToPerform.Move;
        return (NodeState.SUCCESS, Action.None);
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        _rootInitialized = true;

        return (RootNode)currentNode;
    }
}
