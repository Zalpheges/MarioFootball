using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private TreeV3 behaviorTree = new TreeV3();

    private float shootThreshold = Field.Width / 4;
    private float defendThreshold = Field.Width / 20;
    private float attackThreshold = 0.3f;
    private float markThreshold = Field.Width / 25;
    private float headButtThreshold = 1f;
    private float passAlignementThreshold = 0.6f;
    private float shootAlignementThreshold = 0.9f;
    private float dangerRangeThreshold = 3f;

    private void Start()
    {
        List<float> Thresholds = new List<float>
        {
        shootThreshold,
        defendThreshold,
        attackThreshold,
        headButtThreshold,
        markThreshold,
        passAlignementThreshold,
        shootAlignementThreshold,
        dangerRangeThreshold
        };

        behaviorTree.Setup(Allies, Enemies, this.Player, Thresholds);
    }

    private void Update()
    {
        //behaviorTree
    }

    public override Action GetAction()
    {
        return behaviorTree.root.Evaluate().Item2;
    }
}
