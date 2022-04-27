using BehaviorTree;
using UnityEngine;

public class S_WanderAroundPosition : Node
{
    private RootNode _root;
    private bool _rootInitialized = false;

    public override (NodeState, Action) Evaluate()
    {
        if (!_rootInitialized)
            _root = GetRootNode();

        Vector2Int PlayerCoord = _root.PlayerCoordinates;
        Vector3 CenterPosition = new Vector3(PlayerCoord.x * _root.WidthDivision, 0, PlayerCoord.y * _root.HeightDivision);

        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        float randomRadius = Random.Range(0f, 0.7f);

        //Debug.Log($"{_root.player.transform.GetSiblingIndex()}, {randomX}, {randomY}, {randomRadius}");

        Vector2 CenterOffset = new Vector2(randomX, randomY).normalized * randomRadius * Mathf.Min(_root.HeightDivision, _root.WidthDivision) / 2;

        //Debug.Log($"{CenterOffset.x}, {CenterOffset.y}");

        _root.CoordinatePosition = new Vector3(CenterOffset.x, 0, CenterOffset.y) + CenterPosition;
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

