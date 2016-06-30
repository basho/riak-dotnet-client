namespace RiakClient.Commands.TS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Erlang;
    using Messages;

    /// <summary>
    /// Fetches timeseries data from Riak
    /// </summary>
    [CLSCompliant(false)]
    public class Query : Command<QueryOptions, QueryResponse>, IRiakTtbCommand
    {
        private const string TsQueryReqAtom = "tsqueryreq";
        private const string TsInterpolationAtom = "tsinterpolation";
        private const string UndefinedAtom = "undefined";

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

            // NB: always stream, collect results unless callback is passed.
            req.stream = true;

            return req;
        }

        public Action<MemoryStream> GetSerializer()
        {
            return (ms) =>
            {
                using (var os = new OtpOutputStream())
                {
                    os.WriteByte(OtpExternal.VersionTag);
                    // TsQueryReq is a 4-tuple: {'tsqueryreq', TsInterpolation, boolIsStreaming, bytesCoverContext}
                    os.WriteTupleHead(4);
                    os.WriteAtom(TsQueryReqAtom);

                    // TsInterpolation is a 3-tuple
                    // {'tsinterpolation', query, []} empty list is interpolations
                    os.WriteTupleHead(3);
                    os.WriteAtom(TsInterpolationAtom);
                    os.WriteStringAsBinary(CommandOptions.Query);
                    os.WriteNil();

                    os.WriteBoolean(false);
                    os.WriteAtom(UndefinedAtom);

                    os.WriteTo(ms);
                }
            };
        }

        public override void OnSuccess(RpbResp response)
        {
            DecodedResponse dr = null;
            ResponseDecoder decoder = null;

            var ttbresp = response as TsTtbResp;
            if (ttbresp == null)
            {
                decoder = new ResponseDecoder((TsQueryResp)response);
            }
            else
            {
                decoder = new ResponseDecoder(ttbresp);
            }

            dr = decoder.Decode();

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
