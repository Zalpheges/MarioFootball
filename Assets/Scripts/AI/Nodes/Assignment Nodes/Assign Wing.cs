using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class AssignWing : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    private List<Player> _Allies = new List<Player>();

    public override (NodeState, Action) Evaluate()
    {
        if (!rootInitialized)
            _root = GetRootNode();

        _Allies.Clear();
        _Allies.AddRange(_root.parentTree.Allies);

        float highestZ = 0f;
        float lowestZ = 0f;

        Player highestPlayer = null;
        Player lowestPlayer = null;

        bool initializationComplete = false;

        foreach(Player player in _Allies)
        {
            if (!initializationComplete)
            {
                highestPlayer = player;
                highestZ = player.transform.position.z;
                lowestPlayer = player;
                lowestZ = player.transform.position.z;
                initializationComplete = true;
            }
            else 
            {
                if(player.transform.position.z < lowestZ)
                {
                    lowestZ = player.transform.position.z;
                    lowestPlayer = player;
                }
                else if(player.transform.position.z > highestZ)
                {
                    highestZ = player.transform.position.z;
                    highestPlayer = player;
                }
            }
        }

        if (_root.parentTree.player == highestPlayer)
            _root.currentWing = Wing.left;
        else if (_root.parentTree.player == lowestPlayer)
            _root.currentWing = Wing.right;
        else
            _root.currentWing = Wing.center;

        return (NodeState.SUCCESS, Action.None);
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
