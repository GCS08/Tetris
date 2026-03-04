using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract node in a linked data structure, encapsulating a value and a reference to the next node.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the node.</typeparam>
    public abstract class NodeModel<T>
    {
        #region Fields
        protected T? value;
        protected Node<T>? next;
        #endregion
     
        #region Public Methods
        public abstract T GetValue();
        public abstract Node<T> GetNext();
        public abstract void SetNext(Node<T> next);
        #endregion
    }
}
