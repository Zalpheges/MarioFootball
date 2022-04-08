using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override (NodeState, Action) Evaluate()
        {
            foreach (Node node in Children)
            {
                (NodeState, Action) evaluation = node.Evaluate();

                switch (evaluation.Item1)
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return evaluation;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return evaluation;
                }
            }

            return base.Evaluate();
        }
    }
}