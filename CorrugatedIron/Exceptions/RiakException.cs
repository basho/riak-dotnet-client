using System;
using System.Collections;
using System.Collections.Generic;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Exceptions
{
    public class RiakException : Exception
    {
        public uint ErrorCode { get; private set; }
        public string ErrorMessage { get; set; }

        public RiakException(uint errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public override string Message
        {
            get
            {
                return "Riak returned an error. Code '{0}'. Message: {1}".Fmt(ErrorCode, ErrorMessage);
            }
        }

        public override IDictionary Data
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { "ErrorCode", ErrorCode },
                    { "ErrorMessage", ErrorMessage }
                };
            }
        }
    }
}
