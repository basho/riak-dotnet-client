using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using CorrugatedIron.Exceptions;

namespace CorrugatedIron.Comms
{
    public class RiakPbcClientSocket : IDisposable
    {
        private class ReceiveData
        {
            public List<byte[]> Data { get; private set; }
            public Exception Error { get; set; }
            public ManualResetEvent Signal { get; private set; }

            public ReceiveData()
            {
                Data = new List<byte[]>();
                Signal = new ManualResetEvent(false);
            }
        }

        private const int InitialReceiveBufferSize = ushort.MaxValue;
        private readonly string _host;
        private readonly int _port;
        private readonly int _waitTimeout;
        private readonly object _socketLock = new object();
        private Socket _socket;
        private bool _disposing;
        private byte[] _receiveBuffer;
        private bool _connected;

        public RiakPbcClientSocket(string host, int port, int waitTimeout)
        {
            _host = host;
            _port = port;
            _waitTimeout = waitTimeout;
        }

        public bool IsConnected
        {
            get { return _connected; }
        }

        public void Connect()
        {
            if (_connected) return;

            lock(_socketLock)
            {
                _receiveBuffer = _receiveBuffer ?? new byte[InitialReceiveBufferSize];
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // let any exceptions bubble up
                _socket.Connect(_host, _port);
                _connected = true;
            }
        }

        public bool Send(byte[] data)
        {
            Connect();
            return _socket.Send(data) == data.Length;
        }

        public byte[] Receive()
        {
            var recData = new ReceiveData();
            _socket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnReceive, recData);
            if (!recData.Signal.WaitOne(_waitTimeout))
            {
                throw new RiakSocketException(SocketError.TimedOut);
            }

            if (recData.Error != null)
            {
                throw recData.Error;
            }

            return recData.Data.SelectMany(d => d).ToArray();
        }

        private void OnReceive(IAsyncResult result)
        {
            var recData = (ReceiveData)result.AsyncState;
            try
            {
                var bytesRead = _socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    var chunk = new byte[bytesRead];
                    Array.Copy(_receiveBuffer, chunk, bytesRead);
                    recData.Data.Add(chunk);

                    // there may be more, so get the rest
                    if (bytesRead == _receiveBuffer.Length)
                    {
                        _socket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnReceive, recData);
                        return;
                    }
                }

                // all the data is here, so signal that we're done.
                recData.Signal.Set();
            }
            catch (Exception ex)
            {
                recData.Error = ex;
                recData.Signal.Set();
            }
        }

        public void Dispose()
        {
            if (_disposing) return;

            _disposing = true;
            Disconnect();
        }

        public void Disconnect()
        {
            if (!_connected) return;

            lock (_socketLock)
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket.Dispose();
                    _socket = null;
                }
                _connected = false;
            }
        }
    }
}
