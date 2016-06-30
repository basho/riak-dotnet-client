namespace RiakClient.Commands
{
    using System;
    using System.IO;
    using Messages;
    using ProtoBuf;

    /// <summary>
    /// Base class for Riak commands that don't have options.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class Command<TResponse> : IRiakCommand
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TResponse}"/> class.
        /// </summary>
        public Command()
        {
        }

        /// <summary>
        /// A sub-class instance of <see cref="Response"/> representing the response from Riak.
        /// </summary>
        public TResponse Response { get; protected set; }

        public abstract MessageCode ExpectedCode { get; }

        public abstract RiakReq ConstructRequest();

        public abstract void OnSuccess(RpbResp rpbResp);

        public virtual RpbResp DecodeResponse(byte[] buffer)
        {
            Type expectedType = MessageCodeTypeMapBuilder.GetTypeFor(ExpectedCode);

            if (buffer == null || buffer.Length == 0)
            {
                return Activator.CreateInstance(expectedType) as RpbResp;
            }
            else
            {
                using (var memStream = new MemoryStream(buffer))
                {
                    return Serializer.NonGeneric.Deserialize(expectedType, memStream) as RpbResp;
                }
            }
        }
    }
}