using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class NodeModel<T>
    {
        protected T? value;
        protected Node<T>? next;
        public abstract T GetValue();
        public abstract Node<T> GetNext();
        public abstract void SetNext(Node<T> next);

    }
}
