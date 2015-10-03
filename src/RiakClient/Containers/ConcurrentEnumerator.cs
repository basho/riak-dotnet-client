namespace RiakClient.Containers
{
    using System.Collections.Generic;

    internal sealed class ConcurrentEnumerator<T> : IConcurrentEnumerator<T>
    {
        private readonly object lockObj = new object();
        private readonly IEnumerator<T> wrapped;

        public ConcurrentEnumerator(IEnumerator<T> wrapped)
        {
            this.wrapped = wrapped;
        }

        public bool TryMoveNext(out T next)
        {
            lock (lockObj)
            {
                if (wrapped.MoveNext())
                {
                    next = wrapped.Current;
                    return true;
                }

                next = default(T);
                return false;
            }
        }
    }
}