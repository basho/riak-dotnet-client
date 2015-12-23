namespace RiakClient.Commands
{
    using System;
    using Messages;
    using Riak;
    using Riak.Core;

    /// <summary>
    /// Base class for Riak commands that don't have options.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class Command<TResponse> : IRCommand
        where TResponse : Response
    {
        private RError error;
        private Node lastNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TResponse}"/> class.
        /// </summary>
        public Command()
        {
        }

        /// <summary>
        /// An instance of <see cref="RError"/> representing an error from Riak, if any.
        /// </summary>
        public RError Error
        {
            get { return error; }
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// An instance of <see cref="Response"/> representing the response from Riak.
        /// </summary>
        public TResponse Response { get; protected set; }

        // TODO CLIENTS-621
        public Node LastNode
        {
            get { return lastNode; }
        }

        public abstract MessageCode RequestCode { get; }

        public abstract MessageCode ResponseCode { get; }

        public abstract Type ResponseType { get; }

        public abstract RpbReq ConstructPbRequest();

        public abstract void OnSuccess(RpbResp rpbResp);

        public void OnError(RError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.error = error;
        }

        public void SetLastNode(Node lastNode)
        {
            this.lastNode = lastNode;
        }
    }
}
