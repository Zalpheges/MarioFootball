using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public enum SearchType
{
    None,
    Allies,
    Enemies,
    PlayerSpecific
}

public class BallOwnership : Node
{
    private AITree _parentTree;

    private SearchType _searchType;

    public BallOwnership(AITree parentTree, SearchType iSearchType)
    {
        _parentTree = parentTree;
        _searchType = iSearchType;
    }

    public BallOwnership(AITree parentTree, SearchType iSearchType, Node child)
    {
        _parentTree = parentTree;
        _searchType = iSearchType;

        _Attach(child);
    }

    public override NodeState Evaluate()
    {
        switch (_searchType)
        {
            case SearchType.None:
                if (!SearchBallInAllies() && !SearchBallInEnemies())
                    return ValueToReturn();
                break;
            case SearchType.Allies:
                if(SearchBallInAllies())
                    return ValueToReturn();
                break;
            case SearchType.Enemies:
                if(SearchBallInEnemies())
                    return ValueToReturn();
                break;
            case SearchType.PlayerSpecific:
                if(_parentTree.gameObject.GetComponent<Player>().HasBall)
                    return ValueToReturn();
                break;
        }

        return NodeState.FAILURE;
    }

    private NodeState ValueToReturn()
    {
        if (children.Count != 0)
            return children[0].Evaluate();
        return NodeState.SUCCESS;
    }

    private bool SearchBallInAllies()
    {
        foreach (Player player in _parentTree.Allies)
            if (player.HasBall)
                return true;
        return false;
    }

    private bool SearchBallInEnemies()
    {
        foreach (Player player in _parentTree.Enemies)
            if (player.HasBall)
                return true;
        return false;
    }
}
