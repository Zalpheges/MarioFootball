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
    public float passAlignmentThreshold;
    public float shootAlignmentThreshold;
    public float dangerRangeThreshold;

    public bool EastTeamEnabled;
    public bool WestTeamEnabled;

    public int playerIndex;

    public RootNode root;

    public void Setup(Team iAllies, Team iEnemies, Player iplayer, List<float> Thresholds)
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
        passAlignmentThreshold = Thresholds[5];
        shootAlignmentThreshold = Thresholds[6];
        dangerRangeThreshold = Thresholds[7];

        EastTeamEnabled = true;
        WestTeamEnabled = true;

        root = new RootNode(this, new List<Node>()
        {
            new Sequence(new List<Node>()
            {
                new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        new T_TeamSide_East(),
                        new T_EastEnabled(),
                    }),
                    new Sequence(new List<Node>
                    {
                        new T_TeamSide_West(),
                        new T_WestEnabled()
                    })
                }),
                new S_UpdateBallHolder(),
                new Sequence(new List<Node>
                {
                    new Inverter(new T_AITeam()),
                    new Selector(new List<Node>
                    {
                        new T_isPilotedUnchanged(),
                        new Sequence(new List<Node>
                        {
                            new S_UpdatePilotedPlayer(),
                            new S_BallState_Unassigned(),
                            new S_PlayerType_Unassigned()
                        })
                    })
                }),
                new Selector(new List<Node>
                {
                    #region Enemy Team Has Ball
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
                                new S_Marking()
                            }),
                            new Sequence(new List<Node>
                            {
                                new T_InRangeHeadButt(),
                                new S_HeadButt()
                            }),
                            new S_ContestBall()
                        })
                    }),
                    #endregion
                    #region Ally Team Has Ball
                    new Sequence(new List<Node>
                    {
                        new T_BallHolderIsAlly(),
                        new Selector(new List<Node>
                        {
                            new T_BallState_Ally(),
                            #region Variables Reset Upon GameState Switch
                            new Sequence(new List<Node>
                            {
                                new S_GameState_Attack(),
                                new S_BallState_Ally(),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallHolderIsMe(),
                                        new S_PlayerType_BallHolder()
                                    }),
                                    new Sequence(new List<Node>
                                    {
                                        new S_PlayerType_Unassigned(),
                                        new S_ClearPosition()
                                    })
                                })
                            }),
                            #endregion
                        }),
                        new Selector(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new T_PlayerType_BallHolder(),
                                #region AI Striker has Ball
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallHolder_ShootRange(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_GoalUncovered(),
                                                new S_Shoot()
                                            }),
                                            new Sequence(new List<Node>
                                            {
                                                new T_NearbyAllyUnmarked(),
                                                new S_Pass()
                                            }),
                                            new Sequence(new List<Node>
                                            {
                                                new Selector(new List<Node>
                                                {
                                                    new T_BallHolder_TopThird(),
                                                    new T_BallHolder_MiddleThird()
                                                }),
                                                new S_MoveBallHolderDown()
                                            }),
                                            new S_MoveBallHolderUp()
                                        })
                                    }),
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_BallHolder_enemyInRange(),
                                            new T_NearbyAllyUnmarked(),
                                            new S_Pass(),
                                        }),
                                        new CoucouNode(),
                                        new S_MoveBallHolderToGoal()
                                    })
                                })
                                #endregion
                            }),
                            new Sequence(new List<Node>
                            {
                                new Selector(new List<Node>
                                {
                                    new Inverter(new T_PlayerType_Unassigned()),
                                    #region Assign Player types
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_LowestOrder(),
                                            new S_PlayerType_Defender()
                                        }),
                                        new Sequence(new List<Node>
                                        {
                                            new T_HighestOrder(),
                                            new S_PlayerType_Attacker_Bot()
                                        }),
                                        new S_PlayerType_Attacker_Top()
                                    })
                                    #endregion
                                }),
                                new Selector(new List<Node>
                                {
                                    new T_PlayerType_Attacker_Top(),
                                    new T_PlayerType_Attacker_Bot()
                                }),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new T_TeamSide_West(),
                                        #region West Team AI Attackers Placement
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_BallHolder_FourthQuarter(),
                                                new Selector(new List<Node>
                                                {
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_BottomThird(),
                                                        new S_MoveAttacker_4_Bot_West()
                                                    }),
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_MiddleThird(),
                                                        new S_MoveAttacker_4_Mid_West()
                                                    }),
                                                    new S_MoveAttacker_4_Top_West()
                                                })
                                            }),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_BottomThird(),
                                                    new S_MoveAttacker_Bot_West()
                                                }),
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_MiddleThird(),
                                                    new S_MoveAttacker_Mid_West()
                                                }),
                                                new S_MoveAttacker_Top_West()
                                            })
                                        })
                                        #endregion
                                    }),
                                    new Selector(new List<Node>
                                    {
                                        #region East Team AI Attackers Placement
                                        new Sequence(new List<Node>
                                        {
                                            new T_BallHolder_FirstQuarter(),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_BottomThird(),
                                                    new S_MoveAttacker_1_Bot_East()
                                                }),
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_MiddleThird(),
                                                    new S_MoveAttacker_1_Mid_East()
                                                }),
                                                new S_MoveAttacker_1_Top_East()
                                            })
                                        }),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_BallHolder_BottomThird(),
                                                new S_MoveAttacker_Bot_East()
                                            }),
                                            new Sequence(new List<Node>
                                            {
                                                new T_BallHolder_MiddleThird(),
                                                new S_MoveAttacker_Mid_East()
                                            }),
                                            new S_MoveAttacker_Top_East()
                                        })
                                        #endregion
                                    })
                                })
                            }),
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new T_TeamSide_West(),
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_BallHolder_FirstQuarter(),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_BottomThird(),
                                                    new S_Defender_GoalRange_Bottom_West()
                                                }),
                                                new S_Defender_GoalRange_TopCenter_West()
                                            })
                                        }),
                                        new Sequence(new List<Node>
                                        {
                                            new T_BallHolder_SecondQuarter(),
                                            new S_Defender_SecondQuarter_West()
                                        }),
                                        new S_Defender_EnemyHalf_West()
                                    }),
                                }),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallHolder_FourthQuarter(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_BallHolder_BottomThird(),
                                                new S_Defender_GoalRange_Bottom_East()
                                            }),
                                            new S_Defender_GoalRange_TopCenter_East()
                                        })
                                    }),
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallHolder_ThirdQuarter(),
                                        new S_Defender_ThirdQuarter_East(),
                                    }),
                                    new S_Defender_EnemyHalf_East()
                                })
                            })
                        })
                    }),
                    #endregion
                    #region No Team Has Ball
                    new Sequence(new List<Node>
                    {
                        new Selector(new List<Node>
                        {
                            new T_BallState_None(),
                            new Sequence(new List<Node>
                            {
                                new S_BallState_None(),
                                new S_ResetBallSeeker(),
                                new S_Static(),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallSeekerIsMe(),
                                        new S_PlayerType_BallSeeker()
                                    }),
                                    new S_PlayerType_Supporter()
                                }),
                                new Sequence(new List<Node>
                                {
                                    new T_PlayerType_Seeker(),
                                })
                            })
                        })

                    })
                    #endregion
                }),
                #region Perform Action
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
                #endregion
            })
        });
    }
}
