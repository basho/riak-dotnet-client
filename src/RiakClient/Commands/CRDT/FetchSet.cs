namespace RiakClient.Commands.CRDT
{
    using System.Collections.Generic;
    using Exceptions;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchSet : FetchCommand<SetResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchSet"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchSetOptions"/></param>
        public FetchSet(FetchSetOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new SetResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.SET)
                {
                    throw new RiakException(
                        string.Format("Requested set, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new SetResponse();
                }
                else
                {
                    Response = new SetResponse(
                        Options.Key,
                        fetchResp.context,
                        new HashSet<byte[]>(fetchResp.value.set_value));
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<Builder, FetchSet, FetchSetOptions, SetResponse>
        {
        }
    }
}
