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
    public class Query : Command<QueryOptions, QueryResponse>
    {
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

        public override MessageCode ExpectedCode
        {
            get { return MessageCode.TsQueryResp; }
        }

        public override RpbReq ConstructPbRequest()
        {
            var req = new TsQueryReq();

            req.query = new TsInterpolation
            {
                @base = CommandOptions.Query
            };

            // TODO
            req.stream = false;

            return (RpbReq)req;
        }

        public override void OnSuccess(RpbResp response)
        {
            var decoder = new ResponseDecoder(response);
            DecodedResponse dr = decoder.Decode();

            Response = new QueryResponse(CommandOptions.Query, dr.Columns, dr.Rows);
        }

        /// <inheritdoc />
        public class Builder
            : TimeseriesCommandBuilder<Builder, Query, QueryOptions>
        {
            private string query;

            public Builder WithQuery(string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentNullException("query", "query is required and must not be empty.");
                }

                this.query = query;
                return this;
            }

            protected override void PopulateOptions(QueryOptions options)
            {
                options.Query = query;
            }
        }
    }
}
