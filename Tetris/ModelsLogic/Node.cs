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
            return this.value!;
        }

        // Getter for next node
        public override Node<T> GetNext()
        {
            return this.next!;
        }

        // Setter for value
        public override void SetValue(T value)
        {
            this.value = value;
        }

        // Setter for next node
        public override void SetNext(Node<T> next)
        {
            this.next = next;
        }

        // Check if there is a next node
        public override bool HasNext()
        {
            return this.next != null;
        }

        // Override ToString method to display the node's value
        public override string ToString()
        {
            return this.value!.ToString()!;
        }
    }
}
