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

        public bool IsEmpty()
        {
            return first == null;
        }

        public void Insert(T value)
        {
            Node<T> oldLast = last!;
            last = new Node<T>(value);
            if (IsEmpty())
            {
                first = last;
            }
            else
            {
                oldLast.SetNext(last);
            }
        }

        public T Remove()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty, returning default value");
                return default!;
            }
            T value = first!.GetValue();
            first = first.GetNext();
            if (IsEmpty())
            {
                last = null;
            }
            return value;
        }

        public T Head()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty, returning default value");
                return default!;
            }
            return first!.GetValue();
        }
    }
}
