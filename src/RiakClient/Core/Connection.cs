namespace Riak.Core
{
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using RiakClient.Commands;

    internal class Connection : IDisposable
    {
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        private readonly StateManager sm;
        private readonly ConnectionOptions opts;

        private bool disposed = false;
        private ConnectionState finalState;

        private TcpClient client;
        private bool inFlight = false;
        private DateTime lastUsed = DateTime.Now;

        public Connection(ConnectionOptions opts)
        {
            if (opts == null)
            {
                throw new ArgumentNullException("opts");
            }

            this.sm = StateManager.FromEnum<ConnectionState>(sync);
            this.opts = opts;

            client = new TcpClient(AddressFamily.InterNetwork);
            client.NoDelay = true;
            client.ReceiveTimeout = (int)opts.RequestTimeout.TotalMilliseconds;
            client.SendTimeout = client.ReceiveTimeout;

            sm.State = (byte)ConnectionState.Created;
        }

        public enum ConnectionState : byte
        {
            // NB: order matters!
            Created,
            TlsStarting,
            Active,
            Inactive
        }

        public ConnectionState State
        {
            get
            {
                return disposed ? finalState : (ConnectionState)sm.State;
            }
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
                        sm.IsStateLessThan((byte)ConnectionState.Inactive);
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

        public async Task Connect()
        {
            await client.ConnectAsync(opts.Address.Address, opts.Address.Port);
            sm.State = (byte)ConnectionState.Active;
        }

        public void Close()
        {
            Dispose();
        }

        public async Task<Result> Execute(IRiakCommand command)
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
                if (client != null)
                {
                    client.Close();
                    client = null;
                }

                finalState = ConnectionState.Inactive;
                sm.State = (byte)finalState;
                sm.Dispose();

                sync.Dispose();
                disposed = true;
            }
        }

        private void StartExecute(IRiakCommand command)
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
    }
}
