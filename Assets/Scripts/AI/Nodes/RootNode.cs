using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    unassigned,
    Enemy,
    EnemyWithBall,
}
public enum PlayerType
{
    unassigned,
    BallSeeker,
    Contender,
    Supporter,
    Defender,
    BallHolder,
    Receiver
}
public enum BallState
{
    unassigned,
    Ally,
    Enemy,
    Goal,
    None
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
    Load,
    HeadButt
}

public class RootNode : Node
{
    public TreeV4 parentTree;
    public GoalKeeperTree goalParentTree;

    public Player player = null;
    public Player pilotedPlayer = null;

    public Player allyGoalKeeper = null;
    public Player enemyGoalKeeper = null;

    public Player target = null;
    public Player passTarget = null;
    public Player ballContender = null;
    public Player ballHolder = null;
    public Player previousBallHolder = null;
    public Player ballSeeker = null;

    public Vector3 Position = Vector3.zero;
    public Vector3 CoordinatePosition = Vector3.zero;

    public Vector3 InitialGoalPosition;

    public bool AITeam = true;

    public ActionToPerform actionToPerform = ActionToPerform.unassigned;
    public TargetType currentTargetType = TargetType.unassigned;
    public PlayerType currentPlayerType = PlayerType.unassigned;
    public BallState currentBallState = BallState.unassigned;
    public TeamSide allyTeamSide = TeamSide.unassigned;

    public List<Player> Allies = new List<Player>();
    public List<Player> Enemies = new List<Player>();

    public Dictionary<int, Player> allyPlayersOrder = new Dictionary<int, Player>();
    public Dictionary<int, Player> enemyPlayersOrder = new Dictionary<int, Player>();

    public Vector2Int PreviousBallHolderCoordinates = new Vector2Int();
    public Vector2Int BallHolderCoordinates = new Vector2Int();

    public Vector2Int PreviousBallSeekerCoordinates = new Vector2Int();
    public Vector2Int BallSeekerCoordinates = new Vector2Int();

    public Vector2Int PlayerCoordinates = new Vector2Int();
    public Vector2Int[] PlayersCoordinates = new Vector2Int[4];

    public int playerIndex = new int();
    public int ModifyingCoeff = new int();

    public float TimeLoad = 0f;

    public float WidthDivision;
    public float HeightDivision;

    public float shootThreshold;
    public float defenseThreshold;
    public float attackThreshold;
    public float headButtThreshold;
    public float markThreshold;
    public float passAlignmentThreshold;
    public float shootAlignmentThreshold;
    public float dangerRangeThreshold;
    public float randomMaxValue;

    public int WidthDivisionAmount = 9;
    public int HeightDivisionAmount = 7;

    public List<Vector2Int> OptimalCoordinates = new List<Vector2Int>();

    public RootNode(TreeV4 iparentTree, List<Node> ichildren)
    {
        parentTree = iparentTree;
        player = parentTree.player;

        Allies = parentTree.Allies;
        Enemies = parentTree.Enemies;

        allyGoalKeeper = iparentTree.allyGoalKeeper;
        enemyGoalKeeper = iparentTree.enemyGoalKeeper;

        InitializeThresholds(iparentTree.Thresholds);
        randomMaxValue = 1f + 100f * (2 - GameManager.Difficulty);

        WidthDivision = Field.Width / WidthDivisionAmount;
        HeightDivision = Field.Height / HeightDivisionAmount;
        AllyPlayersOrderSetup();
        EnemyPlayersOrderSetup();
        AIBoolSetup();

        if (parentTree.allyGoalTransform.position.x < parentTree.enemyGoalTransform.position.x)
            allyTeamSide = TeamSide.West;
        else
            allyTeamSide = TeamSide.East;

        ModifyingCoeff = allyTeamSide == TeamSide.West ? 1 : -1;

        foreach (Node child in ichildren)
            _Attach(child);
    }
    public RootNode(GoalKeeperTree iparentTree, List<Node> ichildren)
    {
        goalParentTree = iparentTree;
        player = goalParentTree.player;

        passAlignmentThreshold = 0.9f;
        headButtThreshold = 1f;

        Allies = goalParentTree.Allies;
        Enemies = goalParentTree.Enemies;

        InitialGoalPosition = goalParentTree.InitialGoalPosition;

        if (goalParentTree.allyGoalTransform.position.x < goalParentTree.enemyGoalTransform.position.x)
            allyTeamSide = TeamSide.West;
        else
            allyTeamSide = TeamSide.East;

        foreach (Node child in ichildren)
            _Attach(child);
    }

    public void InitializeThresholds(List<float> Thresholds)
    {
        shootThreshold = Thresholds[0];
        defenseThreshold = Thresholds[1];
        attackThreshold = Thresholds[2];
        headButtThreshold = Thresholds[3];
        markThreshold = Thresholds[4];
        passAlignmentThreshold = Thresholds[5];
        shootAlignmentThreshold = Thresholds[6];
        dangerRangeThreshold = Thresholds[7];
    }

    private void AIBoolSetup()
    {
        foreach (Player player in parentTree.Allies)
            if (player.IsPiloted)
                AITeam = false;
    }

    private void AllyPlayersOrderSetup()
    {
        int i = 0;
        foreach (Player player in parentTree.Allies)
            allyPlayersOrder.Add(i++, player);

        return;

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
        int i = 0;
        foreach (Player player in parentTree.Enemies)
            enemyPlayersOrder.Add(i++, player);

        return;

        int highestIndex = 0;
        foreach (Player player in parentTree.Enemies)
            if (highestIndex < player.transform.GetSiblingIndex())
                highestIndex = player.transform.GetSiblingIndex();

        int lowestIndex = highestIndex - parentTree.Enemies.Count + 1;
        foreach (Player player in parentTree.Enemies)
            enemyPlayersOrder.Add(player.transform.GetSiblingIndex() - lowestIndex, player);
    }

    public override (NodeState, Action) Evaluate()
    {
        if (Children != null)
            return Children[0].Evaluate();

        return base.Evaluate();
    }
}
