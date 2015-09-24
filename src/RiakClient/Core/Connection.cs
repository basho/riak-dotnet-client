namespace Riak.Core
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using RiakClient.Commands;
    using RiakClient.Exceptions;

    internal enum ConnectionState
    {
        Created,
        TlsStarting,
        Active,
        Inactive
    }

    internal class Connection : IDisposable
    {
        private readonly object sync = new object();
        private readonly ConnectionOptions opts;
        private readonly TcpClient client;

        private ConnectionState state;
        private bool inFlight = false;
        private DateTime lastUsed = DateTime.Now;

        public Connection(ConnectionOptions opts)
        {
            if (opts == null)
            {
                throw new ArgumentNullException("opts");
            }

            this.opts = opts;

            client = new TcpClient(AddressFamily.InterNetwork);
            client.NoDelay = true;
            client.ReceiveTimeout = (int)opts.RequestTimeout;
            client.SendTimeout = client.ReceiveTimeout;

            SetState(ConnectionState.Created);
        }

        public DateTime LastUsed
        {
            get { return lastUsed; }
        }

        public ConnectionState State
        {
            get
            {
                lock (sync)
                {
                    return state;
                }
            }
        }

        public async Task Connect()
        {
            await client.ConnectAsync(opts.Address, opts.Port);
            SetState(ConnectionState.Active);
        }

        public void Close()
        {
            client.Close();
            SetState(ConnectionState.Inactive);
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
                string errorMessage = string.Format(
                    Properties.Resources.Riak_Core_ConnectionCantWriteException_fmt, opts.Address, opts.Port);
                throw new ConnectionWriteException(errorMessage);
            }
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Close();
            }
        }

        private void SetState(ConnectionState state)
        {
            lock (sync)
            {
                this.state = state;
            }
        }

        private void StartExecute(IRiakCommand command)
        {
            lock (sync)
            {
                if (inFlight)
                {
                    var msg = string.Format(Properties.Resources.Riak_Core_ConnectionInFlightException_fmt, command.GetType().Name);
                    throw new ConnectionInFlightException(msg);
                }

                inFlight = true;
                lastUsed = DateTime.Now;
            }
        }

        private void StopExecute()
        {
            lock (sync)
            {
                inFlight = false;
            }
        }
    }
}
