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
                    new Sequence(new List<Node>
                    {
                        new T_BallHolderIsAlly(),
                        new Selector(new List<Node>
                        {
                            new T_BallState_Ally(),
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
                        }),
                        new Selector(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new T_PlayerType_BallHolder(),
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
                                                new A_Shoot()
                                            }),
                                            new Sequence(new List<Node>
                                            {
                                                new T_NearbyAllyUnmarked(),
                                                new A_Pass()
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
                                            new A_Pass(),
                                        }),
                                        new S_MoveBallHolderToGoal()
                                    })
                                })
                            }),
                            new Sequence(new List<Node>
                            {
                                new Selector(new List<Node>
                                {
                                    new Inverter(new T_PlayerType_Unassigned()),
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
                                            new S_PlayerType_AttackerRight()
                                        }),
                                        new S_PlayerType_AttackerLeft()
                                    })
                                }),
                                new Selector(new List<Node>
                                {
                                    new T_PlayerType_Attacker_Left(),
                                    new T_PlayerType_Attacker_Right()
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
                                                new T_PlayerType_Attacker_Left(),
                                                new Selector(new List<Node>
                                                {
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_FourthQuarter(),
                                                        new Selector(new List<Node>
                                                        {
                                                            new Sequence(new List<Node>
                                                            {
                                                                new T_BallHolder_MiddleThird(),
                                                                new S_AttackerLeft_ShootRange_Center_West()
                                                            }),
                                                            new Sequence(new List<Node>
                                                            {
                                                                new T_BallHolder_BottomThird(),
                                                                new S_AttackerLeft_ShootRange_Bottom_West()
                                                            }),
                                                            new S_AttackerLeft_ShootRange_Top_West()
                                                        })
                                                    }),
                                                    new Selector(new List<Node>
                                                    {
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_MiddleThird(),
                                                            new S_AttackerLeft_Center_West()
                                                        }),
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_BottomThird(),
                                                            new S_AttackerLeft_Bottom_West()
                                                        }),
                                                        new S_AttackerLeft_Top_West()
                                                    })
                                                })
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
                                                                new T_BallHolder_MiddleThird(),
                                                                new S_AttackerRight_ShootRange_Center_West()
                                                            }),
                                                            new Sequence(new List<Node>
                                                            {
                                                                new T_BallHolder_BottomThird(),
                                                                new S_AttackerRight_ShootRange_Bottom_West()
                                                            }),
                                                            new S_AttackerRight_ShootRange_Top_West()
                                                        })
                                                    }),
                                                    new Selector(new List<Node>
                                                    {
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_MiddleThird(),
                                                            new S_AttackerRight_Center_West()
                                                        }),
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_BottomThird(),
                                                            new S_AttackerRight_Bottom_West()
                                                        }),
                                                        new S_AttackerRight_Top_West()
                                                    })
                                                })
                                        })
                                    }),
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_PlayerType_Attacker_Left(),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_FirstQuarter(),
                                                    new Selector(new List<Node>
                                                    {
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_MiddleThird(),
                                                            new S_AttackerLeft_ShootRange_Center_East()
                                                        }),
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_BallHolder_BottomThird(),
                                                            new S_AttackerLeft_ShootRange_Bottom_East()
                                                        }),
                                                        new S_AttackerLeft_ShootRange_Top_East()
                                                    })
                                                }),
                                                new Selector(new List<Node>
                                                {
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_MiddleThird(),
                                                        new S_AttackerLeft_Center_East()
                                                    }),
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_BottomThird(),
                                                        new S_AttackerLeft_Bottom_East()
                                                    }),
                                                    new S_AttackerLeft_Top_East()
                                                })
                                            })
                                        }),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_BallHolder_FirstQuarter(),
                                                new Selector(new List<Node>
                                                {
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_MiddleThird(),
                                                        new S_AttackerRight_ShootRange_Center_East()
                                                    }),
                                                    new Sequence(new List<Node>
                                                    {
                                                        new T_BallHolder_BottomThird(),
                                                        new S_AttackerRight_ShootRange_Bottom_East()
                                                    }),
                                                    new S_AttackerRight_ShootRange_Top_East()
                                                })
                                            }),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_MiddleThird(),
                                                    new S_AttackerRight_Center_East()
                                                }),
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallHolder_BottomThird(),
                                                    new S_AttackerRight_Bottom_East()
                                                }),
                                                new S_AttackerLeft_Top_East()
                                            })
                                        })
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
