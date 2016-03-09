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
    public class Delete : Command<DeleteOptions, DeleteResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Delete"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="DeleteOptions"/></param>
        public Delete(DeleteOptions options)
            : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.Key == null)
            {
                throw new ArgumentNullException("options.Key", "options.Key can not be null");
            }

            if (EnumerableUtil.IsNullOrEmpty(options.Key.Cells))
            {
                throw new ArgumentNullException("options.Key.Cells", "options.Key.Cells can not be null or empty");
            }
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsDelResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsDelReq();

            req.table = CommandOptions.Table;

            req.timeoutSpecified = false;
            if (CommandOptions.Timeout.HasValue)
            {
                req.timeout = (uint)CommandOptions.Timeout;
            }

            req.key.AddRange(CommandOptions.Key.ToTsCells());

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new DeleteResponse(false);
            }
            else
            {
                Response = new DeleteResponse();
            }
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Delete, DeleteOptions>
        {
            private Row key;

            public Builder WithKey(Row key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key", "key is required");
                }

                this.key = key;
                return this;
            }

            protected override void PopulateOptions(DeleteOptions options)
            {
                options.Key = key;
                options.Timeout = timeout;
            }
        }
    }
}
