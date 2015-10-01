namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    /// <summary>
    /// Represents a Riak mapreduce result.
    /// </summary>
    public class RiakMapReduceResult : IRiakMapReduceResult
    {
        private readonly IEnumerable<RiakMapReduceResultPhase> phaseResults;

        internal RiakMapReduceResult(IEnumerable<RiakResult<RpbMapRedResp>> response)
        {
            var phases = from r in response
                         group r by r.Value.phase
                         into g
                         select new
                         {
                             Phase = g.Key,
                             Success = g.First().IsSuccess,
                             PhaseResults = g.Select(rr => rr.Value)
                         };

            phaseResults = phases.OrderBy(p => p.Phase).Select(p => p.Success ? new RiakMapReduceResultPhase(p.Phase, p.PhaseResults) : new RiakMapReduceResultPhase()).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<RiakMapReduceResultPhase> PhaseResults
        {
            get { return phaseResults; }
        }
    }
}
