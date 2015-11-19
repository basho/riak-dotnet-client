namespace Riak.Core
{
    using System;
    using System.IO;
    using System.Net.Sockets;

    internal static class ExceptionExtensions
    {
        public static bool Temporary(this Exception ex)
        {
            var ioex = ex as IOException;
            if (ioex != null)
            {
                var sockex = ioex.InnerException as SocketException;
                if (sockex != null)
                {
                    switch ((SocketError)sockex.ErrorCode)
                    {
                        // TODO 3.0 CLIENTS-606, CLIENTS-621 ADD TO THIS LIST
                        case SocketError.TimedOut:
                            return true;
                        default:
                            return false;
                    }
                }
            }

            return false;
        }
    }
}
