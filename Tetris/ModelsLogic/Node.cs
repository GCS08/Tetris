using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a generic node in a singly linked list, holding a value of type <typeparamref name="T"/> 
    /// and a reference to the next node.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the node.</typeparam>
    public class Node<T> : NodeModel<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class with a specified value.
        /// The next node reference is set to <c>null</c> by default.
        /// </summary>
        /// <param name="value">The value to store in the node.</param>
        public Node(T value)
        {
            this.value = value;
            this.next = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class with a specified value 
        /// and a reference to the next node.
        /// </summary>
        /// <param name="value">The value to store in the node.</param>
        /// <param name="next">The next node in the linked list.</param>
        public Node(T value, Node<T> next)
        {
            this.value = value;
            this.next = next;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value stored in this node.
        /// </summary>
        /// <returns>The value of type <typeparamref name="T"/>. Returns <c>default</c> if the value is <c>null</c>.</returns>
        public override T GetValue()
        {
            if (this.value == null)
                return default!;
            return this.value;
        }

        /// <summary>
        /// Gets the next node in the linked list.
        /// </summary>
        /// <returns>
        /// The next <see cref="Node{T}"/>. Returns <c>default</c> if there is no next node.
        /// </returns>
        public override Node<T> GetNext()
        {
            if (this.next == null)
                return default!;
            return this.next;
        }

        /// <summary>
        /// Sets the reference to the next node in the linked list.
        /// </summary>
        /// <param name="next">The node to set as the next node.</param>
        public override void SetNext(Node<T> next)
        {
            this.next = next;
        }

        #endregion
    }
}