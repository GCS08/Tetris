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
            first = null;
            last = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the queue is empty.
        /// </summary>
        /// <returns><c>true</c> if the queue contains no elements; otherwise, <c>false</c>.</returns>
        public override bool IsEmpty()
        {
            return first == null;
        }

        /// <summary>
        /// Inserts a new element at the end of the queue.
        /// </summary>
        /// <param name="value">The value to insert into the queue.</param>
        public override void Insert(T value)
        {
            Node<T> oldLast = last!;
            last = new Node<T>(value);

            if (IsEmpty())
                first = last;
            else
                oldLast.SetNext(last);
        }

        /// <summary>
        /// Gets the value at the tail (end) of the queue without removing it.
        /// </summary>
        /// <returns>
        /// The value of the last element in the queue, or <c>default</c> if the queue is empty.
        /// </returns>
        public override T GetTail()
        {
            return last == null || IsEmpty() ? default! : last.GetValue();
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
            if (first != null && !IsEmpty())
            {
                result = first.GetValue();
                first = first.GetNext();
                if (IsEmpty())
                    last = null;
            }
            return result;
        }

        /// <summary>
        /// Returns the value at the head (front) of the queue without removing it.
        /// </summary>
        /// <returns>
        /// The value at the front of the queue, or <c>default</c> if the queue is empty.
        /// </returns>
        public override T Head()
        {
            return first == null || IsEmpty() ? default! : first.GetValue();
        }

        /// <summary>
        /// Returns a string representation of the queue and outputs the count of elements.
        /// </summary>
        /// <param name="counter">Outputs the number of elements in the queue.</param>
        /// <returns>A string representation of the queue with values separated by arrows.</returns>
        public override string PrintQueue(out int counter)
        {
            counter = 0;
            string output = string.Empty;
            ModelsLogic.Queue<T> temp = new();

            while (!IsEmpty())
            {
                T value = Remove();
                if (value != null)
                {
                    output += value.ToString() + TechnicalConsts.SpaceSign
                        + TechnicalConsts.ArrowSignString + TechnicalConsts.SpaceSign;

                    temp.Insert(value);
                    counter++;
                }
            }

            while (!temp.IsEmpty())
                Insert(temp.Remove());

            return output;
        }

        /// <summary>
        /// Sorts the queue assuming <typeparamref name="T"/> is KeyValuePair{string,string} 
        /// and the key represents a Unix timestamp.
        /// </summary>
        /// <returns>A task representing the asynchronous sort operation.</returns>
        /// <remarks>
        /// If <typeparamref name="T"/> is not KeyValuePair{string,string} or the queue is empty, 
        /// the method exits without performing any sorting.
        /// </remarks>
        public override async Task SortByUnixTimestampKeyAsync()
        {
            bool shouldSort = typeof(T) == typeof(KeyValuePair<string, string>) && !IsEmpty();

            if (shouldSort)
            {
                await Task.Run(() =>
                {
                    List<KeyValuePair<string, string>> buffer = [];

                    // Drain queue into buffer
                    while (!IsEmpty())
                    {
                        KeyValuePair<string, string> item = (KeyValuePair<string, string>)(object)Remove()!;
                        buffer.Add(item);
                    }

                    // Sort by Unix timestamp key in ascending order
                    buffer.Sort((a, b) =>
                    {
                        long ta = long.Parse(a.Key);
                        long tb = long.Parse(b.Key);
                        return ta.CompareTo(tb);
                    });

                    // Restore sorted queue
                    foreach (KeyValuePair<string, string> item in buffer)
                        Insert((T)(object)item);
                });
            }
        }

        #endregion
    }
}