using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class AITree : BehaviorTree.Tree
{
    public Player[] Allies;
    public Player[] Enemies;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new BallOwnership(this, SearchType.None, new Sequence(new List<Node>
                {

                })),
                new BallOwnership(this, SearchType.Allies, new Sequence(new List<Node>
                {

                })),
                new BallOwnership(this, SearchType.Enemies, new Sequence(new List<Node>
                {

                })),
            })
        });

        return root;
    }

    public void Initialize(Player[] iAllies, Player[] iEnemies)
    {
        Allies = iAllies;
        Enemies = iEnemies;
    }
}
