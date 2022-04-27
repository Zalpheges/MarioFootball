namespace BehaviorTree
{
    public class Inverter : Node
    {
        public Inverter() : base() { }
        public Inverter(Node child) : base(child) { }

        public override (NodeState, Action) Evaluate()
        {
            (NodeState, Action) evaluation = this.Children[0].Evaluate();

            switch (evaluation.Item1)
            {
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    break;
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    break;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    break;
                default:
                    break;
            }
            return (state, evaluation.Item2);
        }
    }
}
