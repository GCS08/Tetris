using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Node<T> : NodeModel<T>
    {
        // Constructor with value only, next is null by default
        public Node(T value)
        {
            this.value = value;
            this.next = null;
        }

        // Constructor with value and reference to the next node
        public Node(T value, Node<T> next)
        {
            this.value = value;
            this.next = next;
        }

        // Getter for value
        public override T GetValue()
        {
            if (this.value == null)
                return default!;
            return this.value;
        }

        // Getter for next node
        public override Node<T> GetNext()
        {
            if (this.next == null)
                return default!;
            return this.next;
        }

        // Setter for next node
        public override void SetNext(Node<T> next)
        {
            this.next = next;
        }
    }
}
