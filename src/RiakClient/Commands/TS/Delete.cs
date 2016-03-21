namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Util;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    [CLSCompliant(false)]
    public class Delete : ByKeyCommand<Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Delete"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ByKeyOptions"/></param>
        public Delete(ByKeyOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsDelResp; }
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new Response(notFound: true);
            }
            else
            {
                Response = new Response(notFound: false);
            }
        }

        protected override ITsByKeyReq GetByKeyReq()
        {
            return new TsDelReq();
        }

        /// <inheritdoc />
        public class Builder
            : Builder<Delete>
        {
        }
    }
}
