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
                new Selector(new List<Node>{
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
                    new Debug_Success()
                }),
                new Selector(new List<Node>
                {
                    #region Enemy Team Has Ball
                    new Sequence(new List<Node>
                    {
                        new T_BallHolderIsEnemy(),
                        new T_BallHolderUnchanged(),
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
                        new T_BallHolderUnchanged(),
                        new Selector(new List<Node>
                        {
                            new T_BallState_Ally(),
                            #region Variables Reset Upon GameState Switch
                            new Sequence(new List<Node>
                            {
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
                                        new S_MoveBallHolderToGoal()
                                    })
                                })
                                #endregion
                            }),
                            new Sequence(new List<Node>
                            {
                                new S_UpdateCoordinates(),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_BallState_Ally(),
                                            new T_BallHolderCoordinatesUnchanged()
                                        }),
                                        new S_MovePlayer()
                                    }),
                                    new Sequence(new List<Node>
                                    {
                                        new S_DetermineOptimalCoords(),
                                        new S_AssignTargetCoordinates(),
                                        new S_MovePlayer()
                                    })
                                })
                            })
                        }),
                        new S_BallState_Ally()
                    }),
                    #endregion
                    #region No Team Has Ball
                    new Selector(new List<Node>
                    {
                        new Sequence(new List<Node>
                        {
                            new T_BallState_None(),
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new T_PlayerType_Receiver(),
                                    new S_MoveSeeker()
                                }),
                                new Sequence(new List<Node>
                                {
                                    new T_PlayerType_Supporter(),
                                    new S_MoveSupporter()
                                }),
                                new S_MoveSeeker()
                            })
                        }),
                        new Sequence(new List<Node>
                        {
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new T_BallState_Ally(),
                                    new Sequence(new List<Node>
                                    {
                                        new T_PassInProgress(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_PassTargetIsMe(),
                                                new S_PlayerType_Receiver()
                                            }),
                                            new S_PlayerType_Supporter()
                                        })
                                    })
                                }),
                                new Sequence(new List<Node>
                                {
                                    new T_BallState_Enemy(),
                                    new Sequence(new List<Node>
                                    {
                                        new T_PassInProgress(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_PassTargetIsMine(),
                                                new S_PlayerType_BallSeeker()
                                            }),
                                            new S_PlayerType_Supporter()
                                        })
                                    })
                                }),
                                new Sequence(new List<Node>
                                {
                                    new S_AssignSeeker(),
                                    new Selector(new List<Node>
                                    {
                                        new T_PlayerType_Seeker(),
                                        new S_PlayerType_Supporter()
                                    })
                                })
                            }),
                            new S_BallState_None()
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
