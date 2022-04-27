using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        private (NodeState, Action) evaluation;

        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override (NodeState, Action) Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in Children)
            {
                evaluation = node.Evaluate();

                switch (evaluation.Item1)
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return base.Evaluate();
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return (state, evaluation.Item2);
        }
    }
}
