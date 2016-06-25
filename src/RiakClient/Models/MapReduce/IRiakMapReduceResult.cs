namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an interface for mapreduce results.
    /// </summary>
    public interface IRiakMapReduceResult
    {
        /// <summary>
        /// The collection of <see cref="RiakMapReduceResultPhase"/> results, 
        /// one for each phase where the results of the phase were kept.
        /// </summary>
        IEnumerable<RiakMapReduceResultPhase> PhaseResults { get; }
    }
}
