using Tetris.ModelsLogic;

namespace Tetris.Models
{
    /// <summary>
    /// Represents an abstract node in a linked data structure, encapsulating a value and a reference to the next node.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the node.</typeparam>
    public class Node<T>
    {
        #region Properties
        public T Value { get; set; } = default!;
        public Node<T>? Next { get; set; }
        #endregion
    }
}
