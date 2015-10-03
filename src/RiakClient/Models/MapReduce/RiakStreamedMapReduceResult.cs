namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents a Riak streaming mapreduce result.
    /// </summary>
    public class RiakStreamedMapReduceResult : IRiakMapReduceResult
    {
        private readonly IEnumerable<RiakResult<RpbMapRedResp>> responseReader;

        internal RiakStreamedMapReduceResult(IEnumerable<RiakResult<RpbMapRedResp>> responseReader)
        {
            this.responseReader = responseReader;
        }

        /// <inheritdoc/>
        public IEnumerable<RiakMapReduceResultPhase> PhaseResults
        {
            get
            {
                return responseReader.Select(item => item.IsSuccess
                    ? new RiakMapReduceResultPhase(
                            item.Value.phase,
                            new List<RpbMapRedResp>
                            {
                                item.Value
                            })
                    : new RiakMapReduceResultPhase());
            }
        }
    }
}
