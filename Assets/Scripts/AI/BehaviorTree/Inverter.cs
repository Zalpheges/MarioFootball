using System.Collections.Generic;

namespace BehaviorTree
{
    public class Inverter : Node
    {
        public Inverter() : base() { }
        public Inverter(Node child) : base(child) { }

        public override NodeState Evaluate()
        {
            Node node = this.children[0];

                switch (node.Evaluate())
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
            return state;
        }
    }
}
