using System.Collections.Generic;

namespace CorrugatedIron.Collections
{
    public interface IConcurrentEnumerator<T>
    {
        bool TryMoveNext(out T next);
    }

    public sealed class ConcurrentEnumerator<T> : IConcurrentEnumerator<T>
    {
        private readonly object _lock = new object();
        private readonly IEnumerator<T> _wrapped;

        public ConcurrentEnumerator(IEnumerator<T> wrapped)
        {
            _wrapped = wrapped;
        }

        public bool TryMoveNext(out T next)
        {
            lock (_lock)
            {
                if (_wrapped.MoveNext())
                {
                    next = _wrapped.Current;
                    return true;
                }

                next = default(T);
                return false;
            }
        }
    }
}
