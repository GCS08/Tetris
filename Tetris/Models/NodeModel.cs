using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class NodeModel<T>
    {
        protected T? value;
        protected Node<T>? next;
        public abstract T GetValue();
        public abstract Node<T> GetNext();
        public abstract void SetValue(T value);
        public abstract void SetNext(Node<T> next);
        public abstract bool HasNext();
        public abstract override string ToString();

    }
}
