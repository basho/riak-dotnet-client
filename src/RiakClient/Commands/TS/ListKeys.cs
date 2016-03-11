namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    public class ListKeys : Command<ListKeysOptions, ListKeysResponse>
    {
        private readonly List<Row> rows = new List<Row>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListKeys"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ByKeyOptions"/></param>
        public ListKeys(ListKeysOptions options)
            : base(options)
        {
        }

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsListKeysResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsListKeysReq();

            req.table = CommandOptions.Table;

            req.timeoutSpecified = false;
            if (CommandOptions.Timeout != default(Timeout))
            {
                req.timeout = (uint)CommandOptions.Timeout;
            }

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            TsListKeysResp rsp = (TsListKeysResp)response;

            Response = new ListKeysResponse(rsp.keys.Select(tsr => new Row(tsr)));

            if (CommandOptions.Callback != null)
            {
                CommandOptions.Callback(Response);
            }
            else
            {
                rows.AddRange(Response.Value);
            }

            var streamingResponse = response as IRpbStreamingResp;
            if (streamingResponse != null && streamingResponse.done)
            {
                Response = new ListKeysResponse(rows);
            }
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, ListKeys, ListKeysOptions>
        {
            private Action<ListKeysResponse> callback;

            public Builder WithCallback(Action<ListKeysResponse> callback)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException("callback");
                }

                this.callback = callback;
                return this;
            }

            protected override void PopulateOptions(ListKeysOptions options)
            {
                options.Timeout = timeout;
                options.Callback = callback;
            }
        }
    }
}
