namespace Riak
{
    using System;
    using System.Threading;

    internal class RQueue<T> : System.Collections.Generic.Queue<T>
    {
        private static readonly TimeSpan IterateWriteLockTimeout = TimeSpan.FromMilliseconds(125);

        private readonly ReaderWriterLockSlim sync;

        public RQueue(ReaderWriterLockSlim sync)
            : base()
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this.sync = sync;
        }

        public bool Iterate(Func<T, RQIterRslt> onItem, TimeSpan timeout = default(TimeSpan))
        {
            bool iterated = false;

            if (timeout == default(TimeSpan))
            {
                timeout = IterateWriteLockTimeout;
            }

            if (iterated = sync.TryEnterWriteLock(timeout))
            {
                try
                {
                    for (int items = this.Count; items > 0; items--)
                    {
                        T item = this.Dequeue();

                        RQIterRslt rslt = onItem(item);

                        if (rslt.ReQueue)
                        {
                            this.Enqueue(item);
                        }

                        if (rslt.Break)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    sync.ExitWriteLock();
                }
            }

            return iterated;
        }
    }
}
