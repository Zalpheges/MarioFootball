using System.Linq;
using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private TreeV2 behaviorTree = new TreeV2();

    public float shootThreshold = Field.Width / 4;
    public float defendThreshold = Field.Width / 15;
    public float attackThreshold = Field.Width / 20;

    private void Start()
    {
        behaviorTree.Setup(Allies, Enemies, this.Player, shootThreshold, defendThreshold, attackThreshold);
    }
    public override Action GetAction()
    {
        return behaviorTree.root.Evaluate().Item2;
    }
}
