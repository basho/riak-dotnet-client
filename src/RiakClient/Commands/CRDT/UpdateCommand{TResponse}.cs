namespace RiakClient.Commands.CRDT
{
    using System;
    using Messages;
    using Util;

    public abstract class UpdateCommand<TResponse> : Command<UpdateCommandOptions, TResponse>
        where TResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand{TResponse}"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="UpdateMapOptions"/></param>
        public UpdateCommand(UpdateCommandOptions options)
            : base(options)
        {
            if (this.CommandOptions.HasRemoves &&
                EnumerableUtil.IsNullOrEmpty(this.CommandOptions.Context))
            {
                throw new InvalidOperationException("When doing any removes a context must be provided.");
            }
        }

        public override MessageCode RequestCode
        {
            get { return MessageCode.DtUpdateReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.DtUpdateResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(DtUpdateResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new DtUpdateReq();

            req.type = CommandOptions.BucketType;
            req.bucket = CommandOptions.Bucket;
            req.key = CommandOptions.Key;

            req.w = CommandOptions.W;
            req.pw = CommandOptions.PW;
            req.dw = CommandOptions.DW;

            req.return_body = CommandOptions.ReturnBody;

            req.timeout = (uint)CommandOptions.Timeout.TotalMilliseconds;

            req.context = CommandOptions.Context;
            req.include_context = CommandOptions.IncludeContext;

            if (req.include_context)
            {
                req.return_body = true;
            }

            req.op = GetRequestOp();

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = default(TResponse);
            }
            else
            {
                Response = CreateResponse(GetKey(response), (DtUpdateResp)response);
            }
        }

        protected abstract DtOp GetRequestOp();

        protected abstract TResponse CreateResponse(RiakString key, DtUpdateResp response);
    }
}
