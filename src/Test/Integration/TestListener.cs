namespace Test.Integration
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;
    using Riak.Core;
    using RiakClient.Messages;

    public class TestListener : IDisposable
    {
        public static readonly Func<TcpClient, Task<bool>> ReadWritePingOnConnAsync = async (c) =>
            {
                bool success = await ReadWritePingRespAsync(c, false);
                if (success)
                {
                    return false;
                }

                return true;
            };

        private static readonly ILog Log = LogManager.GetLogger<TestListener>();

        private static readonly Random R = new Random((int)DateTime.Now.Ticks);

        private readonly TcpListener listener;
        private readonly ushort port;

        private readonly Func<TcpClient, bool> onConn;
        private readonly Func<TcpClient, Task<bool>> onConnAsync;
        private readonly Task listenerTask;
        private readonly AutoResetEvent evt = new AutoResetEvent(false);

        private bool running = true;

        public TestListener(Func<TcpClient, bool> onConn = null, Func<TcpClient, Task<bool>> onConnAsync = null, ushort port = default(ushort))
        {
            this.port = port;

            if (this.port == default(ushort))
            {
                this.port = (ushort)R.Next(default(ushort), ushort.MaxValue);
            }

            listener = new TcpListener(IPAddress.Loopback, this.port);

            this.onConn = onConn;
            this.onConnAsync = onConnAsync;

            listenerTask = Start();

            Log.DebugFormat("listening on {0}:{1}", IPAddress.Loopback, this.port);
        }

        public IPEndPoint EndPoint
        {
            get { return (IPEndPoint)listener.LocalEndpoint; }
        }

        public static async Task<bool> ReadWritePingRespAsync(TcpClient client, bool shouldClose)
        {
            // TODO 3.0 do anything with data read?
            // TODO 3.0 exception handling?
            Stream s = client.GetStream();

            await MessageReader.ReadPbMessageAsync(s);

            await MessageWriter.SerializeAndStreamAsync(null, MessageCode.RpbPingResp, s);

            if (shouldClose)
            {
                client.Close();
            }

            return true;
        }

        public bool WaitOnConn()
        {
            return WaitOnConn(TimeSpan.FromSeconds(1));
        }

        public bool WaitOnConn(TimeSpan timeout)
        {
            return evt.WaitOne(timeout);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                evt.Dispose();
            }
        }

        private async Task Start()
        {
            listener.Start();

            while (running)
            {
                using (TcpClient client = await listener.AcceptTcpClientAsync())
                {
                    // NB: the behavior of this case is quite different than the async case
                    // TODO 3.0 simplify, combine, rename, whatever to make this clearer
                    if (onConn == null && onConnAsync == null)
                    {
                        break;
                    }
                    else if (onConn != null)
                    {
                        if (onConn(client))
                        {
                            evt.Set();
                            break;
                        }
                    }
                    else if (onConnAsync != null)
                    {
                        while (true)
                        {
                            if (await onConnAsync(client))
                            {
                                evt.Set();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Stop()
        {
            running = false;
            listener.Stop();
        }
    }
}
