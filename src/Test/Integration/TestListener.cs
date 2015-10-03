namespace Test.Integration
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NUnit.Framework;
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

        private static readonly Random R = new Random((int)DateTime.Now.Ticks);

        private readonly TcpListener listener;
        private readonly ushort port;

        private readonly Func<TcpClient, bool> onConn;
        private readonly Func<TcpClient, Task<bool>> onConnAsync;

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
        }

        public IPEndPoint EndPoint
        {
            get { return (IPEndPoint)listener.LocalEndpoint; }
        }

        public async Task Start()
        {
            listener.Start();

            using (TcpClient client = await listener.AcceptTcpClientAsync())
            {
                while (true)
                {
                    if (onConn == null && onConnAsync == null)
                    {
                        break;
                    }

                    if (onConn != null && onConn(client))
                    {
                        break;
                    }

                    if (onConnAsync != null && await onConnAsync(client))
                    {
                        break;
                    }
                }
            }
        }

        public void Wait(Task w)
        {
            Wait(w, TimeSpan.FromSeconds(5));
        }

        public void Wait(Task w, TimeSpan timeout)
        {
            bool completed = w.Wait(timeout);
            Assert.True(completed, "Operation timed out");
        }

        public void Stop()
        {
            listener.Stop();
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
                listener.Stop();
            }
        }

        private static async Task<bool> ReadWritePingRespAsync(TcpClient client, bool shouldClose)
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
    }
}
