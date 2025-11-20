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
        public T GetValue()
        {
            return this.value!;
        }

        // Getter for next node
        public Node<T> GetNext()
        {
            return this.next!;
        }

        // Setter for value
        public void SetValue(T value)
        {
            this.value = value;
        }

        // Setter for next node
        public void SetNext(Node<T> next)
        {
            this.next = next;
        }

        // Check if there is a next node
        public bool HasNext()
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
