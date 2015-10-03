namespace Riak
{
    using System;
    using System.Threading;

    internal class RQueue<T> : System.Collections.Generic.Queue<T>
    {
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

        public void Iterate(Func<T, RQIterRslt> onItem)
        {
            sync.EnterWriteLock();
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
    }
}
