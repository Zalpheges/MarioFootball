using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private TreeV2 behaviorTree = new TreeV2();

    public float shootThreshold = Field.Width / 4;
    public float defendThreshold = Field.Width / 20;
    public float attackThreshold = Field.Width / 10;
    public float markThreshold = Field.Width / 25;
    public float headButtThreshold = 1f;
    public float passAlignementThreshold = 0.8f;
    public float shootAlignementThreshold = 0.9f;
    public float dangerRangeThreshold = 2f;

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
