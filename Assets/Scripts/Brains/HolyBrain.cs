using System.Linq;
using UnityEngine;

public class HolyBrain : PlayerBrain
{
    private TreeV2 behaviorTree = new TreeV2();

    public float shootThreshold = Field.Width / 4;
    public float defendThreshold = Field.Width / 10;
    public float attackThreshold = Field.Width / 10;
    public float markThreshold = Field.Width / 10;
    public float headButtThreshold = 5f;

    private void Start()
    {
        float[] Thresholds = new float[5];
        Thresholds.Append(shootThreshold);
        Thresholds.Append(defendThreshold);
        Thresholds.Append(attackThreshold);
        Thresholds.Append(headButtThreshold);
        Thresholds.Append(markThreshold);
        Debug.Log(Thresholds);

        behaviorTree.Setup(Allies, Enemies, this.Player, Thresholds);
    }
    public override Action GetAction()
    {
        return behaviorTree.root.Evaluate().Item2;
    }
}
