using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class QueueModel<T>
    {
        protected Node<T>? first;
        protected Node<T>? last;
        public abstract bool IsEmpty();
        public abstract void Insert(T value);
        public abstract T GetTail();
        public abstract T Remove();
        public abstract T Head();
        public abstract string PrintQueue(out int counter);
    }
}
