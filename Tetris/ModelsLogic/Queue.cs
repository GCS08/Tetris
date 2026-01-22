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
            return last == null || IsEmpty() ? default! : last.GetValue();
        }
        public override T Remove()
        {
            if (first == null || IsEmpty())
                return default!;
            T value = first.GetValue();
            first = first.GetNext();
            if (IsEmpty())
                last = null;
            return value;
        }
        public override T Head()
        {
            return first == null || IsEmpty() ? default! : first.GetValue();
        }
        public override string PrintQueue(out int counter)
        {
            counter = 0;
            string output = string.Empty;
            ModelsLogic.Queue<T> temp = new();
            while (!IsEmpty())
            {
                T value = Remove();
                if (value == null) return output;
                output += value.ToString() + TechnicalConsts.SpaceSign
                    + TechnicalConsts.ArrowSignString + TechnicalConsts.SpaceSign;
                temp.Insert(value);
                counter++;
            }
            while (!temp.IsEmpty())
                Insert(temp.Remove());
            return output;
        }
        public override async Task SortByUnixTimestampKeyAsync()
        {
            // This method only makes sense for this specific T
            if (typeof(T) != typeof(KeyValuePair<string, string>) || IsEmpty())
                return;

            await Task.Run(() =>
            {
                List<KeyValuePair<string, string>> buffer = [];

                // Drain queue
                while (!IsEmpty())
                {
                    KeyValuePair<string, string> item = (KeyValuePair<string, string>)(object)Remove()!;
                    buffer.Add(item);
                }

                // Sort by Unix timestamp (ascending)
                buffer.Sort((a, b) =>
                {
                    long ta = long.Parse(a.Key);
                    long tb = long.Parse(b.Key);
                    return ta.CompareTo(tb);
                });

                // Restore sorted queue
                foreach (KeyValuePair<string, string> item in buffer)
                {
                    Insert((T)(object)item);
                }
            });
        }

    }
}
