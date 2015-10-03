namespace RiakClient.Containers
{
    using System.Collections.Generic;

    internal class ConcurrentEnumerable<T> : IConcurrentEnumerable<T>
    {
        private readonly IEnumerable<T> wrapped;

        public ConcurrentEnumerable(IEnumerable<T> wrapped)
        {
            this.wrapped = wrapped;
        }

        public IConcurrentEnumerator<T> GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(wrapped.GetEnumerator());
        }
    }
}