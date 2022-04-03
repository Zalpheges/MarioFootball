using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System.Linq;

public class TreeV2
{
    public Transform allyGoalTransform;
    public Transform enemyGoalTransform;

    public List<Player> Allies;
    public List<Player> Enemies;

    public Player player;
    public Player playerWithBall;

    public float shootThreshold;
    public float defenseThreshold;
    public float attackThreshold;
    public float headButtThreshold;
    public float markThreshold;

    public int playerIndex;

    public RootNode root;

    public void Setup(Team iAllies, Team iEnemies, Player iplayer, float[] Thresholds)
    {
        Allies = iAllies.Players.ToList();
        Enemies = iEnemies.Players.ToList();
        allyGoalTransform = iAllies.transform;
        enemyGoalTransform = iEnemies.transform;
        player = iplayer;
        playerIndex = player.transform.GetSiblingIndex();
        shootThreshold = Thresholds[0];
        defenseThreshold = Thresholds[1];
        attackThreshold = Thresholds[2];
        headButtThreshold = Thresholds[3];
        markThreshold = Thresholds[4];

        root = new RootNode(this, new List<Node>()
        {
            new Sequence(new List<Node>()
            {
                new UpdateBallHolder(),
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new T_BallHolderIsEnemy(),
                        new Selector(new List<Node>
                        {
                            new T_BallState_Enemy(),
                            new Sequence(new List<Node>
                            {
                                new S_GameState_Defend(),
                                new S_BallState_Enemy(),
                                new S_ResetTarget()
                            })
                        }),
                        new Selector(new List<Node>
                        {
                            new T_TargetInitialized(),
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new T_AITeam(),
                                    new S_SetContender(),
                                    new T_ContenderIsMe(),
                                    new S_TargetContenderAssignment(),
                                    new S_TargetType_EnemyWithBall(),
                                    new S_PlayerType_Contender()
                                }),
                                new Sequence(new List<Node>
                                {
                                    new S_TargetAssignment(),
                                    new S_TargetType_Enemy(),
                                    new S_PlayerType_Defender()
                                })
                            })
                        }),
                        new Selector(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new T_PlayerType_Defender(),
                                new S_Marking(),
                                new CoucouNode(),
                            }),
                            new Sequence(new List<Node>
                            {
                                new T_InRangeHeadButt(),
                                new S_HeadButt()
                            }),
                            new S_ContestBall()
                        })
                    })
                }),
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new T_ActionType_Move(),
                        new A_Move()
                    }),
                    new Sequence(new List<Node>
                    {
                        new T_ActionType_HeadButt(),
                        new A_HeadButt()
                    }),
                    new Sequence(new List<Node>
                    {
                        new T_ActionType_Shoot(),
                        new A_Shoot()
                    }),
                    new Sequence(new List<Node>
                    {
                        new T_ActionType_Pass(),
                        new A_Pass()
                    })
                })
            })
        });
    }
}
