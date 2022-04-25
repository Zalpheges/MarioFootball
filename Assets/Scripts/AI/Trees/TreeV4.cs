using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System.Linq;

public class TreeV4
{
    public Transform allyGoalTransform;
    public Transform enemyGoalTransform;

    public List<Player> Allies;
    public List<Player> Enemies;

    public Player allyGoalKeeper;
    public Player enemyGoalKeeper;

    public Player player;
    public Player playerWithBall;

    public List<float> Thresholds;

    public bool EastTeamEnabled;
    public bool WestTeamEnabled;

    public int playerIndex;

    public RootNode root;

    public void Setup(Team iAllies, Team iEnemies, Player iplayer, List<float> iThresholds)
    {
        Allies = iAllies.Players.ToList();
        Enemies = iEnemies.Players.ToList();

        allyGoalKeeper = iAllies.Goalkeeper;
        enemyGoalKeeper = iEnemies.Goalkeeper;

        allyGoalTransform = iAllies.transform;
        enemyGoalTransform = iEnemies.transform;

        player = iplayer;
        playerIndex = player.transform.GetSiblingIndex();

        Thresholds = iThresholds;

        EastTeamEnabled = true;
        WestTeamEnabled = true;

        root = new RootNode(this, new List<Node>()
        {
            new Sequence(new List<Node>()
            {
                #region Verify if AI is Enabled
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
                #endregion
                new S_UpdateBallHolder(),
                new Selector(new List<Node>
                {
                    #region Verify If Player Switched
                    new T_AITeam(),
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
                    #endregion
                }),
                new Selector(new List<Node>
                {
                    #region GoalKeeper Has Ball
                    new Sequence(new List<Node>
                    {
                        new Selector(new List<Node>
                        {
                            new T_AllyGoalKeeperHasBall(),
                            new T_EnemyGoalKeeperHasBall()
                        }),
                        new S_UpdateCoordinates(),
                        new Selector(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new T_BallState_Goal(),
                                new Selector(new List<Node>
                                {
                                    new Sequence(new List<Node>
                                    {
                                        new CoucouNode(),
                                        new T_PositionReached(),
                                        new S_WanderAroundPosition()
                                    }),
                                    new S_MovePlayer()
                                })
                            }),
                            new Sequence(new List<Node>
                            {
                                new S_BallState_Goal(),
                                new S_DetermineOptimalCoords_Goal(),
                                new S_AssignTargetCoordinates_Goal(),
                                new S_MovePlayer()
                            })
                        })
                    }),
                    #endregion
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
                        #region Variables Reset Upon GameState Switch
                        new T_BallHolderIsAlly(),
                        new T_BallHolderUnchanged(),
                        new Selector(new List<Node>
                        {
                            new T_BallState_Ally(),
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
                        }),
                        #endregion
                        new Selector(new List<Node>
                        {
                            #region AI Striker has Ball
                            new Sequence(new List<Node>
                            {
                                new T_PlayerType_BallHolder(),
                                new Selector(new List<Node>
                                {
                                    #region Loading to Shoot
                                    new Sequence(new List<Node>
                                    {
                                        new T_ActionType_Load(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_Loading_Complete(),
                                                new S_Shoot()
                                            }),
                                            new S_Load()
                                        })
                                    }),
                                    #endregion
                                    #region Deciding for action in Shoot Range
                                    new Sequence(new List<Node>
                                    {
                                        new T_BallHolder_ShootRange(),
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_GoalUncovered(),
                                                new S_ResetLoadTime(),
                                                new S_Load()
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
                                    #endregion
                                    #region Deciding for action in the rest of the Field
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
                                    #endregion
                                })
                            }),
                            #endregion
                            #region AI Moves According to the BallHolder
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
                                        new Selector(new List<Node>
                                        {
                                            new Sequence(new List<Node>
                                            {
                                                new T_PositionReached(),
                                                new S_WanderAroundPosition()
                                            }),
                                            new S_MovePlayer()
                                        })

                                    }),
                                    new Sequence(new List<Node>
                                    {
                                        new S_DetermineOptimalCoords(),
                                        new S_AssignTargetCoordinates_Ally(),
                                        new S_MovePlayer()
                                    })
                                })
                            })
                        #endregion
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
                                    new S_Static()
                                }),
                                new Sequence(new List<Node>
                                {
                                    new T_BallSeekerNotDebuffed(),
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_PlayerType_Supporter(),
                                            new S_UpdateCoordinates(),
                                            new S_UpdateBallSeekerCoordinates(),
                                            new Selector(new List<Node>
                                            {
                                                new Sequence(new List<Node>
                                                {
                                                    new T_BallSeekerCoordinatesUnchanged(),
                                                    new Selector(new List<Node>
                                                    {
                                                        new Sequence(new List<Node>
                                                        {
                                                            new T_PositionReached(),
                                                            new S_WanderAroundPosition()
                                                        }),
                                                        new S_MovePlayer()
                                                    })                                                   
                                                }),
                                                new Sequence(new List<Node>
                                                {
                                                    new S_DetermineOptimalCoords(),
                                                    new S_AssignTargetCoordinates_None()
                                                })
                                            })
                                        }),
                                        new S_MoveSeeker()
                                    })
                                })
                            })
                        }),
                        new Sequence(new List<Node>
                        {
                            new Selector(new List<Node>
                            {
                                new Sequence(new List<Node>
                                {
                                    new T_PassInProgress(),
                                    new Selector(new List<Node>
                                    {
                                        new Sequence(new List<Node>
                                        {
                                            new T_PassTargetIsMe(),
                                            new S_PlayerType_Receiver(),
                                            new S_Static()
                                        }),
                                        new Sequence(new List<Node>
                                        {
                                            new T_PassTargetIsMine(),
                                            new S_PlayerType_BallSeeker()
                                        }),
                                        new Sequence(new List<Node>
                                        {
                                            new S_PlayerType_Supporter(),
                                            new S_UpdateCoordinates(),
                                            new S_UpdateBallSeekerCoordinates(),
                                            new S_DetermineOptimalCoords(),
                                            new S_AssignTargetCoordinates_None()
                                        })
                                    })
                                }),
                                new Sequence(new List<Node>
                                {
                                    new S_AssignSeeker(),
                                    new Selector(new List<Node>
                                    {
                                        new T_PlayerType_Seeker(),
                                        new Sequence(new List<Node>
                                        {
                                            new S_PlayerType_Supporter(),
                                            new S_UpdateCoordinates(),
                                            new S_UpdateBallSeekerCoordinates(),
                                            new S_DetermineOptimalCoords(),
                                            new S_AssignTargetCoordinates_None()
                                        })
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
                    }),
                    new Sequence(new List<Node>
                    {
                        new T_ActionType_Load(),
                        new A_Load()
                    })
                })
                #endregion
            })
        });
    }
}
