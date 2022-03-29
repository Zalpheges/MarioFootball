using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class AssignTarget : Node
{
    private RootNode _root;
    private bool rootInitialized = false;

    private List<Player> Allies = new List<Player>();
    private List<Player> Enemies = new List<Player>();

    public override (NodeState, Action) Evaluate()
    {
        if (!rootInitialized)
            _root = GetRootNode();

        InitializeTeams();

        bool assignmentComplete = false;
        Player closestEnemy;

        do
        {
            closestEnemy = null;
            Player closestAlly = _root.parentTree.player;
            float shortestDistance = 0f;

            float distance = 0f;


            foreach (Player player in Enemies)
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
            foreach (Player player in Allies)
            {
                if (!(player == _root.parentTree.player))
                {
                    distance = (closestEnemy.transform.position - player.transform.position).magnitude;

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestAlly = player;
                    }
                }
            }
            if (closestAlly == _root.parentTree.player)
                assignmentComplete = true;
            else
            {
                Enemies.Remove(closestEnemy);
                Allies.Remove(closestAlly);
            }



        } while (!assignmentComplete);

        _root.currentTargetType = closestEnemy.HasBall ? TargetType.enemyWithBall : TargetType.enemy;
        _root.target = closestEnemy;

        return (NodeState.SUCCESS, Action.None);
    }

    private void InitializeTeams()
    {
        Allies.Clear();
        Enemies.Clear();

        Allies.AddRange(_root.parentTree.Allies);
        Enemies.AddRange(_root.parentTree.Enemies);
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
