namespace RiakClient.Commands.CRDT
{
    using Exceptions;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchMap : FetchCommand<MapResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchMap"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchMapOptions"/></param>
        public FetchMap(FetchMapOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new MapResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.MAP)
                {
                    throw new RiakException(
                        string.Format("Requested map, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new MapResponse();
                }
                else
                {
                    Response = new MapResponse(Options.Key, fetchResp.context, fetchResp.value.map_value);
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<FetchMap.Builder, FetchMap, FetchMapOptions, MapResponse>
        {
        }
    }
}
