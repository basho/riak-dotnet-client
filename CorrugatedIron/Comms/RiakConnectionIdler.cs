using System;

namespace CorrugatedIron.Comms
{
    public class RiakConnectionIdler : IDisposable
    {
        private readonly IRiakConnection _connection;

        public RiakConnectionIdler(IRiakConnection connection)
        {
            _connection = connection;
            _connection.EndIdle();
        }

        public void Dispose()
        {
            _connection.BeginIdle();
        }
    }
}
