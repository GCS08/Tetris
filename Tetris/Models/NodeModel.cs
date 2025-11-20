using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class NodeModel<T>
    {
        protected T? value;
        protected Node<T>? next;
    }
}
