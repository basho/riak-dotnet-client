using System.Collections.Generic;

namespace CorrugatedIron.Collections
{
    public interface IConcurrentEnumerable<T>
    {
        IConcurrentEnumerator<T> GetEnumerator();
    }

    public class ConcurrentEnumerable<T> : IConcurrentEnumerable<T>
    {
        private readonly IEnumerable<T> _wrapped;

        public ConcurrentEnumerable(IEnumerable<T> wrapped)
        {
            _wrapped = wrapped;
        }

        public IConcurrentEnumerator<T> GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(_wrapped.GetEnumerator());
        }
    }
}
