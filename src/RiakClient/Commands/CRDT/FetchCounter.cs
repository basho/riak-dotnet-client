namespace RiakClient.Commands.CRDT
{
    using System;
    using Exceptions;
    using Messages;

    /// <summary>
    /// Fetches a Map from Riak
    /// </summary>
    public class FetchCounter : FetchCommand<CounterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetchCounter"/> class.
        /// </summary>
        /// <param name="options">Options for this operation. See <see cref="FetchCounterOptions"/></param>
        public FetchCounter(FetchCounterOptions options)
            : base(options)
        {
        }

        public override void OnSuccess(RpbResp response)
        {
            if (response == null)
            {
                Response = new CounterResponse();
            }
            else
            {
                DtFetchResp fetchResp = (DtFetchResp)response;
                if (fetchResp.type != DtFetchResp.DataType.COUNTER)
                {
                    throw new RiakException(
                        string.Format("Requested counter, received {0}", fetchResp.type));
                }

                if (fetchResp.value == null)
                {
                    Response = new CounterResponse();
                }
                else
                {
                    Response = new CounterResponse(Options.Key, fetchResp.context, fetchResp.value.counter_value);
                }
            }
        }

        /// <inheritdoc />
        public class Builder
            : FetchCommandBuilder<FetchCounter.Builder, FetchCounter, FetchCounterOptions, CounterResponse>
        {
        }
    }
}
