using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class AssignTarget : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    private List<Player> _Allies = new List<Player>();
    private List<Player> _Enemies = new List<Player>();

    public override (NodeState, Action) Evaluate()
    {
        if (!rootInitialized)
            _root = GetRootNode();

        InitializeTeams();
        _Allies.Remove(_root.parentTree.player);

        bool assignmentComplete = false;
        Player closestEnemy;

        do
        {
            closestEnemy = null;
            Player closestAlly = _root.parentTree.player;
            float shortestDistance = 0f;

            float distance = 0f;


            foreach (Player player in _Enemies)
            {
                distance = (player.transform.position - _root.parentTree.player.transform.position).magnitude;

                if (closestEnemy == null)
                {
                    closestEnemy = player;
                    shortestDistance = distance;
                }

                else
                {
                    if (distance < shortestDistance)
                    {
                        closestEnemy = player;
                        shortestDistance = distance;
                    }
                }
            }
            foreach (Player player in _Allies)
            {
                distance = (closestEnemy.transform.position - player.transform.position).magnitude;

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestAlly = player;
                }
                else if(distance == shortestDistance)
                {
                    if (player.transform.GetSiblingIndex() < _root.parentTree.player.transform.GetSiblingIndex())
                        closestAlly = player;
                }
            }
            if (closestAlly == _root.parentTree.player)
                assignmentComplete = true;
            else
            {
                _Enemies.Remove(closestEnemy);
                _Allies.Remove(closestAlly);
            }



        } while (!assignmentComplete);

        _root.currentTargetType = closestEnemy.HasBall ? TargetType.enemyWithBall : TargetType.enemy;
        _root.target = closestEnemy;

        return (NodeState.SUCCESS, Action.None);
    }

    private void InitializeTeams()
    {
        _Allies.Clear();
        _Enemies.Clear();

        _Allies.AddRange(_root.parentTree.Allies);
        _Enemies.AddRange(_root.parentTree.Enemies);
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
