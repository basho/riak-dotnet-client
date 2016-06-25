namespace RiakClient.Commands.CRDT
{
    using System;
    using Messages;

    /// <summary>
    /// Fetches a CRDT from Riak
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data from Riak.</typeparam>
    public abstract class FetchCommand<TResponse> : Command<FetchCommandOptions, TResponse>
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchCommandOptions"/></param>
        public FetchCommand(FetchCommandOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// The request message code.
        /// </summary>
        public override MessageCode RequestCode
        {
            get { return MessageCode.DtFetchReq; }
        }

        /// <summary>
        /// The expected response message code from Riak.
        /// </summary>
        public override MessageCode ResponseCode
        {
            get { return MessageCode.DtFetchResp; }
        }

        /// <summary>
        /// The expected response type.
        /// </summary>
        public override Type ResponseType
        {
            get { return typeof(DtFetchResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new DtFetchReq();

            req.type = CommandOptions.BucketType;
            req.bucket = CommandOptions.Bucket;
            req.key = CommandOptions.Key;

            req.r = CommandOptions.R;
            req.pr = CommandOptions.PR;

            req.timeout = (uint)CommandOptions.Timeout.TotalMilliseconds;

            req.notfound_ok = CommandOptions.NotFoundOK;
            req.include_context = CommandOptions.IncludeContext;
            req.basic_quorum = CommandOptions.UseBasicQuorum;

            return req;
        }
    }
}
