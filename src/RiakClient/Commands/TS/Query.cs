namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using Messages;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    [CLSCompliant(false)]
    public class Query : Command<QueryOptions, QueryResponse>
    {
        private readonly List<Row> rows = new List<Row>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="ByKeyOptions"/></param>
        public Query(QueryOptions options)
            : base(options)
        {
            if (string.IsNullOrWhiteSpace(options.Query))
            {
                throw new ArgumentNullException("options.Query", "Query must be non-null and non-empty");
            }
        }

        public override MessageCode RequestCode
        {
            get { return MessageCode.TsQueryReq; }
        }

        public override MessageCode ResponseCode
        {
            get { return MessageCode.TsQueryResp; }
        }

        public override Type ResponseType
        {
            get { return typeof(TsQueryResp); }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsQueryReq();

            req.query = new TsInterpolation
            {
                @base = CommandOptions.Query
            };

            // NB: always stream, collect results unless callback is passed.
            req.stream = true;

            return req;
        }

        public override void OnSuccess(RpbResp response)
        {
            var decoder = new ResponseDecoder((TsQueryResp)response);
            DecodedResponse dr = decoder.Decode();

            Response = new QueryResponse(CommandOptions.Query, dr.Columns, dr.Rows);

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
                Response = new QueryResponse(CommandOptions.Query, dr.Columns, rows);
            }
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Query, QueryOptions>
        {
            private string query;
            private Action<QueryResponse> callback;

            public Builder WithQuery(string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentNullException("query", "query is required and must not be empty.");
                }

                this.query = query;
                return this;
            }

            public Builder WithCallback(Action<QueryResponse> callback)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException("callback");
                }

                this.callback = callback;
                return this;
            }

            protected override void PopulateOptions(QueryOptions options)
            {
                options.Query = query;
                options.Callback = callback;
            }
        }
    }
}
