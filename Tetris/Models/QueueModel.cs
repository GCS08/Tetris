using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract generic queue model providing basic queue operations and structure.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public abstract class QueueModel<T>
    {
        #region Fields
        protected Node<T>? first;
        protected Node<T>? last;
        #endregion

        #region Public Methods
        public abstract bool IsEmpty();
        public abstract void Insert(T value);
        public abstract T GetTail();
        public abstract T Remove();
        public abstract T Head();
        public abstract string PrintQueue(out int counter);
        public abstract Task SortByUnixTimestampKeyAsync();
        #endregion
    }
}
