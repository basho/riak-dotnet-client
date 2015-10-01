namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// The collection of results for a single mapreduce phase.
    /// </summary>
    public class RiakMapReduceResultPhase
    {
        private readonly bool success;
        private readonly long phase;
        private readonly IList<byte[]> values;

        internal RiakMapReduceResultPhase(long phase, IEnumerable<RpbMapRedResp> results)
        {
            this.phase = phase;
            this.values = results.Select(r => r.response).Where(b => b != null).ToList();
            this.success = true;
        }

        internal RiakMapReduceResultPhase()
        {
            this.success = false;
        }

        /// <summary>
        /// Indicates whether the phase was a success or not.
        /// <b>true</b> if the phase was a success, <b>false</b>, otherwise.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }

        /// <summary>
        /// The phase number.
        /// </summary>
        public long Phase
        {
            get { return phase; }
        }

        /// <summary>
        /// The collection of raw result values for this phase.
        /// </summary>
        public IList<byte[]> Values
        {
            get { return values; }
        }

        /// <summary>
        /// Deserialize a List of <typeparam name="T">T</typeparam> from the phase results
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>IList<typeparam name="T">T</typeparam></returns>
        public IList<T> GetObjects<T>()
        {
            return Values.Select(v => JsonConvert.DeserializeObject<T>(v.FromRiakString())).ToList();
        }

        /// <summary>
        /// Deserialize a List of <see cref="RiakObjectId"/> from $key query
        /// </summary>
        /// <returns>IList of <see cref="RiakObjectId"/></returns>
        /// <remarks>This is designed specifically to deal with the data structure that is returned from
        /// Riak when querying the $key index. This should be used when querying $key directly or through
        /// one of the convenience methods.</remarks>
        public IList<RiakObjectId> GetObjectIds()
        {
            return Values.SelectMany(v => JsonConvert.DeserializeObject<string[][]>(v.FromRiakString()).Select(
                a => new RiakObjectId(a[0], a[1]))).ToList();
        }
    }
}
