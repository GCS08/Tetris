using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract generic queue model providing basic queue operations and structure.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public abstract class QueueModel<T>
    {
        #region Properties
        public Node<T>? First { get; set; }
        public Node<T>? Last { get; set; }
        #endregion

        #region Public Methods
        public abstract bool IsEmpty();
        public abstract void Insert(T value);
        public abstract T Remove();
        public abstract Task SortByUnixTimestampKeyAsync();
        #endregion
    }
}
