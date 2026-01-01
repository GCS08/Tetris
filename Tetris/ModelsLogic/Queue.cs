using Tetris.Models;

namespace Tetris.ModelsLogic
{
    public class Queue<T> : QueueModel<T>
    {
        public Queue()
        {
            first = null;
            last = null;
        }
        public override bool IsEmpty()
        {
            return first == null;
        }
        public override void Insert(T value)
        {
            Node<T> oldLast = last!;
            last = new Node<T>(value);
            if (IsEmpty())
                first = last;
            else
                oldLast.SetNext(last);
        }
        public override T GetTail()
        {
            return IsEmpty() ? default! : last!.GetValue();
        }
        public override T Remove()
        {
            if (IsEmpty())
                return default!;
            T value = first!.GetValue();
            first = first.GetNext();
            if (IsEmpty())
                last = null;
            return value;
        }
        public override T Head()
        {
            return IsEmpty() ? default! : first!.GetValue();
        }
    }
}
