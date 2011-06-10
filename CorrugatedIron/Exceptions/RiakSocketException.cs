using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Exceptions
{
    public class RiakSocketException : Exception
    {
        public SocketError ErrorCode { get; private set; }

        public RiakSocketException(SocketError errorCode)
        {
            ErrorCode = errorCode;
        }

        public override string Message
        {
            get
            {
                return "The underlying Socket communication failed. {0}".Fmt(ErrorCode);
            }
        }

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "ErrorCode", ErrorCode }
                };
            }
        }
    }
}
