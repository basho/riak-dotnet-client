namespace Riak.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;

    internal class ConnectionManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<ConnectionManager>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly StateManager sm;
        private readonly ConnectionManagerOptions opts;
        private readonly RQueue<Connection> queue;

        private Task expirationTask = null;
        private bool disposed = false;

        private ushort connectionCount = 0;

        public ConnectionManager(ConnectionManagerOptions opts)
        {
            if (opts == null)
            {
                throw new ArgumentNullException("opts", Properties.Resources.Riak_Core_ConnectionManagerRequiresOptionsException);
            }

            this.sm = StateManager.FromEnum<ConnectionManagerState>(sync);
            this.opts = opts;
            this.queue = new RQueue<Connection>(sync);

            sm.State = (byte)ConnectionManagerState.Created;
        }

        public enum ConnectionManagerState : byte
        {
            Created,
            Running,
            ShuttingDown,
            Shutdown,
            Error
        }

        public ushort ConnectionCount
        {
            get
            {
                sync.EnterReadLock();
                try
                {
                    return connectionCount;
                }
                finally
                {
                    sync.ExitReadLock();
                }
            }
        }

        public async Task StartAsync()
        {
            sm.StateCheck((byte)ConnectionManagerState.Created);

            var tasks = new Task<Connection>[opts.MinConnections];
            for (ushort i = 0; i < opts.MinConnections; i++)
            {
                tasks[i] = Create();
            }

            Connection[] connected = await Task.WhenAll(tasks);
            foreach (Connection c in connected)
            {
                queue.Enqueue(c);
            }

            expirationTask = Task.Run((Action)ExpireConnections);

            sm.State = (byte)ConnectionManagerState.Running;

            Log.DebugFormat(Properties.Resources.Riak_Core_Running_fmt, this);
        }

        public void Stop()
        {
            sync.EnterUpgradeableReadLock();
            try
            {
                if (sm.IsCurrentState((byte)ConnectionManagerState.Shutdown, (byte)ConnectionManagerState.Error))
                {
                    Log.WarnFormat(
                        Properties.Resources.Riak_Core_ConnectionManagerStopAlreadyCalled_fmt,
                        this,
                        sm);
                    return;
                }

                sm.StateCheck((byte)ConnectionManagerState.Running);

                Log.DebugFormat(Properties.Resources.Riak_Core_ShuttingDown_fmt, this);

                sm.State = (byte)ConnectionManagerState.ShuttingDown;

                if (connectionCount != queue.Count)
                {
                    Log.ErrorFormat(
                        Properties.Resources.Riak_Core_ConnectionManagerStopConnectionCountNotEqualQueueCount_fmt,
                        this,
                        connectionCount,
                        queue.Count);
                }

                Func<Connection, RQIterRslt> onItem = (Connection c) =>
                {
                    c.Close();
                    connectionCount--;
                    return new RQIterRslt(false, false);
                };

                queue.Iterate(onItem);

                if (expirationTask != null)
                {
                    Task.WaitAll(expirationTask);
                }

                sm.State = (byte)ConnectionManagerState.Shutdown;
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Stop();
                sync.Dispose();
                sm.Dispose();
                disposed = true;
            }
        }

        public override string ToString()
        {
            return opts.Address.ToString();
        }

        private Task<Connection> Create()
        {
            sync.EnterUpgradeableReadLock();
            try
            {
                if (!sm.IsStateLessThan((byte)ConnectionManagerState.ShuttingDown))
                {
                    return null;
                }

                if (connectionCount >= opts.MaxConnections)
                {
                    throw new ConnectionException(Properties.Resources.Riak_Core_ConnectionManagerAllConnectionsInUseException_fmt);
                }

                Task<Connection> conn = CreateConnectionAsync();

                sync.EnterWriteLock();
                try
                {
                    connectionCount++;
                }
                finally
                {
                    sync.ExitWriteLock();
                }

                return conn;
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }
        }

        private async Task<Connection> CreateConnectionAsync()
        {
            var opts = new ConnectionOptions(this.opts.Address, this.opts.ConnectTimeout, this.opts.RequestTimeout);
            Connection conn = new Connection(opts);

            await conn.Connect();

            return conn;
        }

        private Connection Get()
        {
            Connection conn = null;

            Func<Connection, RQIterRslt> onItem = (Connection c) =>
            {
                if (c.Available)
                {
                    conn = c;
                    return new RQIterRslt(@break: true, requeue: false);
                }
                else
                {
                    return new RQIterRslt(@break: false, requeue: true);
                }
            };

            queue.Iterate(onItem);

            return conn;
        }

        private void Put(Connection conn)
        {
            sync.EnterWriteLock();
            try
            {
                if (sm.IsStateLessThan((byte)ConnectionManagerState.ShuttingDown))
                {
                    queue.Enqueue(conn);
                }
                else
                {
                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerReturnedShutdown_fmt, this);

                    connectionCount--;
                    conn.Close();
                }
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        private void Remove(Connection conn)
        {
            sync.EnterReadLock();
            try
            {
                if (sm.IsStateLessThan((byte)ConnectionManagerState.ShuttingDown))
                {
                    sync.EnterWriteLock();
                    try
                    {
                        connectionCount--;
                        conn.Close();
                    }
                    finally
                    {
                        sync.ExitWriteLock();
                    }
                }
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        private void ExpireConnections()
        {
            bool running = true;
            ushort expiredCount = 0;

            Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationRoutineStarting_fmt, this);

            while (running)
            {
                if (sm.IsCurrentState((byte)ConnectionManagerState.Created))
                {
                    // NB: waiting for Start()
                    Thread.Sleep(opts.IdleExpirationInterval);
                }
                else if (sm.IsStateLessThan((byte)ConnectionManagerState.ShuttingDown))
                {
                    DateTime now = DateTime.Now;
                    expiredCount = 0;

                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpiringAt_fmt, this, now);

                    Func<Connection, RQIterRslt> onItem = (Connection c) =>
                    {
                        if (connectionCount > opts.MinConnections)
                        {
                            if (!c.Available || (now.Subtract(c.LastUsed) >= opts.IdleTimeout))
                            {
                                c.Close();
                                connectionCount--;
                                expiredCount++;
                            }

                            return new RQIterRslt(@break: false, requeue: false);
                        }
                        else
                        {
                            return new RQIterRslt(@break: true, requeue: true);
                        }
                    };

                    queue.Iterate(onItem);

                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpiredConnections_fmt, this, expiredCount);

                    Thread.Sleep(opts.IdleExpirationInterval);
                }
                else
                {
                    running = false;
                }
            }

            Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationRoutineStopping_fmt, this);
        }
    }
}