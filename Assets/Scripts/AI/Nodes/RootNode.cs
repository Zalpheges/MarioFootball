using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    unassigned,
    Attack,
    Defend,
}

public enum BallPosition
{
    unassigned,
    AllyHalf,
    EnemyHalf
}

public enum TargetType
{
    unassigned,
    Enemy,
    EnemyWithBall,
}

public enum PlayerType
{
    unassigned,
    Seeker,
    Contender,
    Supporter,
    Attacker_left,
    Attacker_right,
    Defender,
    BallHolder
}
public enum BallState
{
    unassigned,
    Ally,
    Enemy,
    None
}

public enum Positionning
{
    unassigned,
    left,
    right,
    center
}

public enum TeamSide
{
    unassigned,
    West,
    East
}

public enum ActionToPerform
{
    unassigned,
    None,
    Shoot,
    Pass,
    Move,
    HeadButt
}

public class RootNode : Node
{
    public TreeV2 parentTree;

    public Player player = null;

    public Player target = null;
    public Player ballContender = null;
    public Player ballHolder = null;
    public Player ballSeeker = null;

    public Vector3 Position = Vector3.zero;

    public bool AITeam = true;

    public ActionToPerform actionToPerform = ActionToPerform.unassigned;
    public BallPosition currentBallPosition = BallPosition.unassigned;
    public Positionning currentPositionning = Positionning.unassigned;
    public TargetType currentTargetType = TargetType.unassigned;
    public PlayerType currentPlayerType = PlayerType.unassigned;
    public BallState currentBallState = BallState.unassigned;
    public GameState currentGameState = GameState.unassigned;
    public TeamSide allyTeamSide = TeamSide.unassigned;

    public Dictionary<int, Player> allyPlayersOrder = new Dictionary<int, Player>();
    public Dictionary<int, Player> enemyPlayersOrder = new Dictionary<int, Player>();

    public RootNode(TreeV2 iparentTree, List<Node> ichildren)
    {
        parentTree = iparentTree;
        player = parentTree.player;
        AllyPlayersOrderSetup();
        EnemyPlayersOrderSetup();
        AIBoolSetup();

        if (parentTree.allyGoalTransform.position.x < parentTree.enemyGoalTransform.position.x)
            allyTeamSide = TeamSide.West;
        else
            allyTeamSide = TeamSide.East;

        foreach (Node child in ichildren)
            _Attach(child);
    }

    private void AIBoolSetup()
    {
        foreach (Player player in parentTree.Allies)
            if (player.IsPiloted)
                AITeam = false;

    }

    private void AllyPlayersOrderSetup()
    {
        int highestIndex = 0;
        foreach (Player player in parentTree.Allies)
            if (highestIndex < player.transform.GetSiblingIndex())
                highestIndex = player.transform.GetSiblingIndex();

        int lowestIndex = highestIndex - parentTree.Allies.Count + 1;
        foreach (Player player in parentTree.Allies)
            allyPlayersOrder.Add(player.transform.GetSiblingIndex() - lowestIndex, player);
    }

    private void EnemyPlayersOrderSetup()
    {
        int highestIndex = 0;
        foreach (Player player in parentTree.Enemies)
            if (highestIndex < player.transform.GetSiblingIndex())
                highestIndex = player.transform.GetSiblingIndex();

        int lowestIndex = highestIndex - parentTree.Enemies.Count + 1;
        foreach (Player player in parentTree.Enemies)
            enemyPlayersOrder.Add(player.transform.GetSiblingIndex() - lowestIndex,player);
    }

    public override (NodeState, Action) Evaluate()
    {
        if (this.Children != null)
            return Children[0].Evaluate();

        return base.Evaluate();
    }
}
