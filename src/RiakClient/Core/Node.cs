namespace Riak.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    public class Node : IDisposable, INode
    {
        private static readonly ILog Log = LogManager.GetLogger<Node>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        // TODO 3.0 should this be passed in by client lib user?
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly CancellationToken ct;

        private readonly NodeOptions opts;
        private readonly ConnectionManager cm;
        private readonly StateManager sm;

        private Task healthCheckTask;
        private bool disposed = false;

        public Node(NodeOptions opts)
        {
            this.opts = opts;
            if (this.opts == null)
            {
                throw new ArgumentNullException("opts", Properties.Resources.Riak_Core_NodeRequiresOptionsException);
            }

            ct = cts.Token;

            var cmo = new ConnectionManagerOptions(
                opts.Address,
                (ushort)opts.MinConnections,
                (ushort)opts.MaxConnections,
                Constants.DefaultIdleExpirationInterval,
                opts.IdleTimeout,
                opts.ConnectTimeout,
                opts.RequestTimeout);
            cm = new ConnectionManager(cmo);

            sm = StateManager.FromEnum<State>(this, sync);
            sm.SetState((byte)State.Created);
        }

        public enum State : byte
        {
            Created = 0,
            Running,
            HealthChecking,
            ShuttingDown,
            Shutdown,
            Error
        }

        public int ExecuteCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public async Task<ExecuteResult> ExecuteAsync(IRCommand cmd)
        {
            ExecuteResult rv = null;

            sm.StateCheck((byte)State.Running, (byte)State.HealthChecking);

            cmd.SetLastNode(this);

            if (sm.IsCurrentState((byte)State.Running))
            {
                Connection conn = cm.Get();

                // TODO 3.0 error getting connection, do health check
                Log.DebugFormat(Properties.Resources.Riak_Core_ExecutingCommand_fmt, this, cmd.Name);

                try
                {
                    // TODO 3.0 CLIENTS-606 Write test to ensure rv has the correct ExecuteResult
                    rv = await conn.ExecuteAsync(cmd);

                    // NB: any errors here do not require closing the connection
                    cm.Put(conn);

                    if (!rv.Executed)
                    {
                        healthCheckTask = Task.Run((Action)DoHealthCheck);
                    }
                }
                catch (Exception ex)
                {
                    // Determine if the connection should be kept open or nuked, then do health check.
                    // timeout errors should be temporary
                    rv = new ExecuteResult(ex);

                    if (ex.Temporary())
                    {
                        cm.Put(conn);
                    }
                    else
                    {
                        conn.Dispose();
                    }

                    healthCheckTask = Task.Run((Action)DoHealthCheck);
                }
            }
            else
            {
                rv = new ExecuteResult(false);
            }

            return rv;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return this.opts.Address.ToString();
        }

        internal async Task StartAsync()
        {
            sm.StateCheck((byte)State.Created);

            Log.DebugFormat(Properties.Resources.Riak_Core_Starting_fmt, this);

            // TODO 3.0 exceptions
            await cm.StartAsync();
            SetState(State.Running);

            Log.DebugFormat(Properties.Resources.Riak_Core_Running_fmt, this);
        }

        internal void Stop()
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

                sm.StateCheck((byte)State.Running, (byte)State.HealthChecking);

                SetState(State.ShuttingDown);

                Log.DebugFormat(Properties.Resources.Riak_Core_ShuttingDown_fmt, this);
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }

            if (healthCheckTask != null)
            {
                Log.DebugFormat(
                    Properties.Resources.Riak_Core_NodeWaitingHealthCheckTask_fmt,
                    this);

                cts.Cancel();

                // TODO 3.0: exceptions in health check task
                if (!Task.WaitAll(new[] { healthCheckTask }, Constants.FiveSeconds))
                {
                    Log.ErrorFormat(
                        Properties.Resources.Riak_Core_NodeStopHealthCheckStopTimeout_fmt,
                        this);
                }
            }
        }

        internal State GetState()
        {
            return (State)sm.GetState();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                Stop();
                cts.Dispose();
                cm.Dispose();
                sm.Dispose();
                sync.Dispose();
                disposed = true;
            }
        }

        protected virtual void SetState(State s)
        {
            sm.SetState((byte)s);
        }

        private void DoHealthCheck()
        {
            // NB: ensure we're not already health checking or shutting down
            sync.EnterUpgradeableReadLock();
            try
            {
                if (sm.IsStateLessThan((byte)State.HealthChecking))
                {
                    SetState(State.HealthChecking);
                    healthCheckTask = Task.Run((Action)HealthCheck);
                }
                else
                {
                    Log.DebugFormat(Properties.Resources.Riak_Core_NodeAlreadyHealthChecking_fmt, this);
                }
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }
        }

        private void HealthCheck()
        {
            Log.DebugFormat(Properties.Resources.Riak_Core_NodeStartingHealthCheck_fmt, this);

            while (ct.IsCancellationRequested == false)
            {
                if (!sm.IsCurrentState((byte)State.HealthChecking))
                {
                    Log.DebugFormat(Properties.Resources.Riak_Core_NodeHealthCheckQuitting_fmt, this, sm);
                    break;
                }

                Log.DebugFormat(Properties.Resources.Riak_Core_NodeRunningHealthCheckAt_fmt, this, DateTime.Now);

                Connection conn = cm.CreateConnection();

                Log.DebugFormat(Properties.Resources.Riak_Core_NodeHealthCheckSleeping_fmt, this, opts.HealthCheckInterval);
                bool cancelled = ct.WaitHandle.WaitOne(opts.HealthCheckInterval);
                if (cancelled)
                {
                    Log.DebugFormat(Properties.Resources.Riak_Core_NodeHealthCheckCancelled_fmt, this);
                    break;
                }
            }
        }
    }
}
