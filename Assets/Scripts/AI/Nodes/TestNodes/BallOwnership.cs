using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public enum SearchType
{
    Allies,
    Enemies,
    PlayerSpecific
}

public class BallOwnership : Node
{
    private SearchType _searchType;
    private RootNode _root;
    private bool rootInitialized = false;

    public BallOwnership(SearchType isearchType)
    {
        _searchType = isearchType;
    }

    public override (NodeState, Action) Evaluate()
    {
        if (!rootInitialized)
        {
            _root = GetRootNode();
            rootInitialized = true;
        }

        switch (_searchType)
        {
            case SearchType.Allies:
                if (SearchBallInTeam(true))
                    return (NodeState.SUCCESS, Action.None);
                break;
            case SearchType.Enemies:
                if (SearchBallInTeam(false))
                    return (NodeState.SUCCESS, Action.None);
                break;
            case SearchType.PlayerSpecific:
                if (_root.parentTree.player.HasBall)
                {
                    _root.currentTargetType = TargetType.allyWithBall;
                    return (NodeState.SUCCESS, Action.None);
                }
                break;
        }
        return base.Evaluate();
    }

    private bool SearchBallInTeam(bool ally)
    {
        List<Player> teamToTest = ally ? _root.parentTree.Allies : _root.parentTree.Enemies;

        foreach (Player player in teamToTest)
        {
            if (player.HasBall)
            {
                _root.parentTree.playerWithBall = player;
                _root.currentGameState = ally ? GameState.attack : GameState.defend;
                return true;
            }
        }

        return false;
    }

    private RootNode GetRootNode()
    {
        Node currentNode = this;

        while (currentNode.parent != null)
            currentNode = currentNode.parent;

        return (RootNode)currentNode;
    }
}
