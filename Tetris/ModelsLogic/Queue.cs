using Tetris.Models;

namespace Tetris.ModelsLogic
{
    /// <summary>
    /// Represents a generic queue data structure with FIFO behavior.
    /// Provides insertion, removal, head/tail access, printing, and type-specific sorting.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public class Queue<T> : QueueModel<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new empty instance of the <see cref="Queue{T}"/> class.
        /// </summary>
        public Queue()
        {
            First = null;
            Last = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the queue is empty.
        /// </summary>
        /// <returns><c>true</c> if the queue contains no elements; otherwise, <c>false</c>.</returns>
        public override bool IsEmpty()
        {
            return First == null;
        }

        /// <summary>
        /// Inserts a new element at the end of the queue.
        /// </summary>
        /// <param name="value">The value to insert into the queue.</param>
        public override void Insert(T value)
        {
            Node<T> oldLast = Last!;
            Last = new Node<T>() { Value = value };

            if (IsEmpty())
                First = Last;
            else
                oldLast.Next = Last;
        }

        /// <summary>
        /// Removes and returns the element at the head (front) of the queue.
        /// </summary>
        /// <returns>
        /// The value of the removed element, or <c>default</c> if the queue is empty.
        /// </returns>
        public override T Remove()
        {
            T result = default!;
            if (First != null && !IsEmpty())
            {
                result = First.Value;
                First = First.Next;
                if (IsEmpty())
                    Last = null;
            }
            return result;
        }
        #endregion
    }
}