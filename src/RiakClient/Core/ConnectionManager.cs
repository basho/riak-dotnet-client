namespace Riak.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;

    internal class ConnectionManager : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<ConnectionManager>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        // TODO 3.0 should this be passed in by client lib user?
        // should it be the Node cts?
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly CancellationToken ct;

        private readonly ConnectionManagerOptions opts;
        private readonly StateManager sm;
        private readonly RQueue<Connection> queue;

        private Task expirationTask = null;
        private bool disposed = false;

        private ushort connectionCount = 0;

        public ConnectionManager(ConnectionManagerOptions opts)
        {
            this.opts = opts;
            if (this.opts == null)
            {
                throw new ArgumentNullException("opts", Properties.Resources.Riak_Core_ConnectionManagerRequiresOptionsException);
            }

            ct = cts.Token;

            queue = new RQueue<Connection>(sync);

            sm = StateManager.FromEnum<State>(this, sync);
            sm.SetState((byte)State.Created);
        }

        public enum State : byte
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

        public State GetState()
        {
            return (State)sm.GetState();
        }

        public async Task StartAsync()
        {
            sm.StateCheck((byte)State.Created);

            var tasks = new Task<Connection>[opts.MinConnections];
            for (ushort i = 0; i < opts.MinConnections; i++)
            {
                tasks[i] = Create();
            }

            // TODO 3.0 what happens if exceptions happen?
            Connection[] connected = await Task.WhenAll(tasks);
            foreach (Connection c in connected)
            {
                queue.Enqueue(c);
            }

            expirationTask = Task.Run((Action)ExpireConnections);

            sm.SetState((byte)State.Running);

            Log.DebugFormat(Properties.Resources.Riak_Core_Running_fmt, this);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "http://blogs.msdn.com/b/tilovell/archive/2014/02/12/the-worst-code-analysis-rule-that-s-recommended-ca2202.aspx")]
        public void Stop()
        {
            sync.EnterUpgradeableReadLock();
            try
            {
                if (sm.IsCurrentState((byte)State.Shutdown, (byte)State.Error))
                {
                    Log.WarnFormat(
                        Properties.Resources.Riak_Core_StopAlreadyCalled_fmt,
                        this,
                        sm);
                    return;
                }

                sm.StateCheck((byte)State.Running);

                Log.DebugFormat(Properties.Resources.Riak_Core_ShuttingDown_fmt, this);

                sm.SetState((byte)State.ShuttingDown);

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
                    c.Dispose();
                    connectionCount--;
                    return new RQIterRslt(false, false);
                };

                if (!queue.Iterate(onItem, Constants.FiveSeconds))
                {
                    Log.ErrorFormat(
                        Properties.Resources.Riak_Core_ConnectionManagerStopIteratingConnectionQueueTimeout_fmt,
                        this,
                        queue.Count);
                }
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }

            if (expirationTask != null)
            {
                Log.DebugFormat(
                    Properties.Resources.Riak_Core_ConnectionManagerWaitingExpirationTask_fmt,
                    this);

                cts.Cancel();

                // TODO 3.0: exceptions in expiration task
                if (!Task.WaitAll(new[] { expirationTask }, Constants.FiveSeconds))
                {
                    Log.ErrorFormat(
                        Properties.Resources.Riak_Core_ConnectionManagerStopExpirationStopTimeout_fmt,
                        this);
                }
            }

            sm.SetState((byte)State.Shutdown);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Stop();
                cts.Dispose();
                sync.Dispose();
                sm.Dispose();
                disposed = true;
            }
        }

        public override string ToString()
        {
            return opts.Address.ToString();
        }

        public Connection Get()
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

        public void Put(Connection conn)
        {
            sync.EnterWriteLock();
            try
            {
                if (sm.IsStateLessThan((byte)State.ShuttingDown))
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

        public void Remove(Connection conn)
        {
            sync.EnterReadLock();
            try
            {
                if (sm.IsStateLessThan((byte)State.ShuttingDown))
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

        public async Task<Connection> CreateConnectionAsync()
        {
            var opts = new ConnectionOptions(this.opts.Address, this.opts.ConnectTimeout, this.opts.RequestTimeout);
            var conn = new Connection(opts);
            await conn.ConnectAsync();
            return conn;
        }

        public Connection CreateConnection()
        {
            var opts = new ConnectionOptions(this.opts.Address, this.opts.ConnectTimeout, this.opts.RequestTimeout);
            var conn = new Connection(opts);
            conn.Connect();
            return conn;
        }

        private Task<Connection> Create()
        {
            sync.EnterUpgradeableReadLock();
            try
            {
                if (!sm.IsStateLessThan((byte)State.ShuttingDown))
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

        private void ExpireConnections()
        {
            ushort expiredCount = 0;

            Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationRoutineStarting_fmt, this);

            while (ct.IsCancellationRequested == false)
            {
                if (sm.IsCurrentState((byte)State.Created))
                {
                    // NB: waiting for Start()
                    Thread.Sleep(opts.IdleExpirationInterval);
                }
                else if (sm.IsStateLessThan((byte)State.ShuttingDown))
                {
                    DateTime now = DateTime.Now;
                    expiredCount = 0;

                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpiringAt_fmt, this, now);

                    Func<Connection, RQIterRslt> onItem = (Connection c) =>
                    {
                        if (ct.IsCancellationRequested)
                        {
                            return new RQIterRslt(@break: true, requeue: true);
                        }

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

                    if (!queue.Iterate(onItem))
                    {
                        Log.DebugFormat(
                            Properties.Resources.Riak_Core_ConnectionManagerIteratingConnectionQueueForExpirationTimeout_fmt,
                            this,
                            queue.Count);
                    }

                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpiredConnections_fmt, this, expiredCount);

                    Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationSleeping_fmt, this, opts.IdleExpirationInterval);
                    bool cancelled = ct.WaitHandle.WaitOne(opts.IdleExpirationInterval);
                    if (cancelled)
                    {
                        Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationCancelled_fmt, this);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionManagerExpirationRoutineStopping_fmt, this);
        }
    }
}
