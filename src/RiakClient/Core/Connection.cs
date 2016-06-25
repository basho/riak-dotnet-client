namespace Riak.Core
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using RiakClient.Commands;

    internal class Connection : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<Connection>();

        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private readonly ConnectionOptions opts;
        private readonly StateManager sm;
        private readonly TcpClient client;

        private bool disposed = false;

        private bool inFlight = false;
        private DateTime lastUsed = DateTime.Now;

        public Connection(ConnectionOptions opts)
        {
            this.opts = opts;
            if (this.opts == null)
            {
                throw new ArgumentNullException("opts");
            }

            this.sm = StateManager.FromEnum<State>(this, sync);

            client = new TcpClient(AddressFamily.InterNetwork);
            client.NoDelay = true;
            client.ReceiveTimeout = (int)opts.RequestTimeout.TotalMilliseconds;
            client.SendTimeout = client.ReceiveTimeout;

            // TODO 3.0 CLIENTS-606, CLIENTS-621
            // http://www.extensionmethod.net/csharp/net/setsocketkeepalivevalues
            // http://www.codeproject.com/Articles/117557/Set-Keep-Alive-Values
            sm.SetState((byte)State.Created);
        }

        public enum State : byte
        {
            // NB: order matters!
            Created,
            TlsStarting,
            Active,
            Inactive
        }

        public DateTime LastUsed
        {
            get { return lastUsed; }
        }

        public bool Available
        {
            get
            {
                sync.EnterReadLock();
                try
                {
                    return client != null &&
                        inFlight == false &&
                        sm.IsStateLessThan((byte)State.Inactive, alreadyLocked: true);
                }
                finally
                {
                    sync.ExitReadLock();
                }
            }
        }

        public override string ToString()
        {
            return this.opts.Address.ToString();
        }

        public State GetState()
        {
            return (State)sm.GetState();
        }

        public async Task ConnectAsync()
        {
            // TODO 3.0 retry on fail?
            await client.ConnectAsync(opts.Address.Address, opts.Address.Port);
            OnConnected();
        }

        public void Connect()
        {
            // TODO 3.0 retry on fail?
            client.Connect(opts.Address.Address, opts.Address.Port);
            OnConnected();
        }

        public void Close()
        {
            client.Close();
            sm.SetState((byte)State.Inactive);
        }

        public async Task<ExecuteResult> ExecuteAsync(IRCommand command)
        {
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                try
                {
                    StartExecute(command);

                    var writer = new MessageWriter(command, stream);
                    await writer.WriteAsync();

                    var reader = new MessageReader(command, stream);
                    return await reader.ReadAsync();
                }
                finally
                {
                    StopExecute();
                }
            }
            else
            {
                // TODO: exception vs inactivate connection / error code?
                var message = string.Format(
                    Properties.Resources.Riak_Core_ConnectionCantWriteException_fmt, opts.Address);
                throw new ConnectionWriteException(message);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Close();
                sm.Dispose();
                sync.Dispose();
                disposed = true;
            }
        }

        private void StartExecute(IRCommand command)
        {
            sync.EnterWriteLock();
            try
            {
                if (inFlight)
                {
                    var msg = string.Format(Properties.Resources.Riak_Core_ConnectionInFlightException_fmt, this, command.GetType().Name);
                    throw new ConnectionInFlightException(msg);
                }

                inFlight = true;
                lastUsed = DateTime.Now;
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        private void StopExecute()
        {
            sync.EnterWriteLock();
            try
            {
                inFlight = false;
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        private void OnConnected()
        {
            Log.DebugFormat(Properties.Resources.Riak_Core_ConnectionConnectedTo_fmt, opts.Address);
            sm.SetState((byte)State.Active);
        }
    }
}
