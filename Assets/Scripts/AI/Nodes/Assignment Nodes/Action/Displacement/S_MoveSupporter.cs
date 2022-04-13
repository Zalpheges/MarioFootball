using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class S_MoveSupporter : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        (int,int) Indexes = RetrieveIndexes();

        int SeekerIndex = Indexes.Item1;
        int PlayerIndex = Indexes.Item2;

        if (SeekerIndex < PlayerIndex)
            PlayerIndex--;

        float defendPositionX = _root.allyTeamSide == TeamSide.West ? - Field.Width / 4 : Field.Width / 4;

        switch (PlayerIndex)
        {
            case 0:
                _root.Position = new Vector3(defendPositionX, 0f, - Field.Height /3);
                break;
            case 1:
                _root.Position = new Vector3(defendPositionX, 0f, 0f);
                break;
            case 2:
                _root.Position = new Vector3(defendPositionX, 0f, Field.Height / 3);
                break;
        }

        _root.actionToPerform = ActionToPerform.Move;
        return (NodeState.SUCCESS, Action.None);
    }

    private (int,int) RetrieveIndexes()
    {
        int SeekerIndex = 0;
        int PlayerIndex = 0;

        for (int x = 0; x < 4; x++)
        {
            if (_root.allyPlayersOrder[x] == _root.ballSeeker)
                SeekerIndex = x;
            if (_root.allyPlayersOrder[x] == _root.player)
                PlayerIndex = x;
        }

        return (SeekerIndex, PlayerIndex);
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

