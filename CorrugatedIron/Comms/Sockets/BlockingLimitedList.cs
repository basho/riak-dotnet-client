using System.Collections.Generic;
using System.Threading;

namespace CorrugatedIron.Comms.Sockets
{
    public class BlockingLimitedList<T>
    {
        private readonly List<T> _list;
        private readonly int _maxSize;

        public BlockingLimitedList(int maxSize)
        {
            _maxSize = maxSize;
            _list = new List<T>(maxSize);
        }

        public void Enqueue(T item)
        {
            lock (_list)
            {
                while (_list.Count >= _maxSize)
                {
                    System.Diagnostics.Debug.WriteLine("Waiting in enqueue as queue size = " + _list.Count);
                    Monitor.Wait(_list);
                 }
                _list.Add(item);
                if (_list.Count > 0)
                {
                    // wake up any blocked dequeue
                    Monitor.PulseAll(_list);
                }
            }
        }

        public bool Dequeue(T item)
        {
            lock (_list)
            {
                var result = _list.Remove(item);
                if (_list.Count < _maxSize)
                {
                    // wake up any blocked enqueue
                    Monitor.PulseAll(_list);
                }
                return result;
            }
        }

        public void Clear()
        {
            while (_list.Count > 0)
            {
                lock (_list)
                {
                    _list.Clear();

                    // wake up any blocked enqueue
                    Monitor.PulseAll(_list);
                    Monitor.Wait(_list);
                }
            }
        }
    }
}
