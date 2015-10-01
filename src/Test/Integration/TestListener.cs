namespace Test.Integration
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class TestListener
    {
        private readonly TcpListener listener;
        private readonly Func<TcpClient, bool> onConn;

        public TestListener(ushort port, Func<TcpClient, bool> onConn)
        {
            this.listener = new TcpListener(IPAddress.Loopback, port);
            this.onConn = onConn;
        }

        public async Task Start()
        {
            listener.Start();
            TcpClient client = await listener.AcceptTcpClientAsync();
            while (true)
            {
                if (onConn(client))
                {
                    break;
                }
            }
        }

        public void Wait(Task w, int timeout = 5000)
        {
            bool completed = w.Wait(timeout);
            Assert.True(completed, "Operation timed out");
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}