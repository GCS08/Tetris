using Tetris.ModelsLogic;

namespace Tetris.Models
{
    public abstract class QueueModel<T>
    {
        protected Node<T>? first;
        protected Node<T>? last;
    }
}
