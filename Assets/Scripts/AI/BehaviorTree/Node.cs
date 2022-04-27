using System.Collections.Generic;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> Children = new List<Node>();

        public Node()
        {
            parent = null;
        }
        public Node(Node child)
        {
            _Attach(child);
        }
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }
        protected void _Attach(Node node)
        {
            node.parent = this;
            Children.Add(node);
        }

        public virtual (NodeState, Action) Evaluate() => (NodeState.FAILURE, Action.None);
    }
}