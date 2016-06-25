namespace RiakClient.Models.RiakDt
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an operation result for a Riak Map data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class RiakDtMapResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtMapResult"/> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="context">The current context, if one was returned with the operation result.</param>
        /// <param name="values">The current collection of <see cref="RiakDtMapEntry"/>s, if they were returned with the operation result.</param>
        public RiakDtMapResult(
            RiakResult<RiakObject> result,
            byte[] context = null,
            List<RiakDtMapEntry> values = null)
        {
            Result = result;

            if (context != null)
            {
                Context = context;
            }

            Values = values ?? new List<RiakDtMapEntry>();
        }

        /// <summary>
        /// The operation result.
        /// </summary>
        public RiakResult<RiakObject> Result { get; private set; }

        /// <summary>
        /// The updated data type context.
        /// </summary>
        public byte[] Context { get; internal set; }

        /// <summary>
        /// A collection of <see cref="RiakDtMapEntry"/>, which represent the updated map fields.
        /// </summary>
        public List<RiakDtMapEntry> Values { get; internal set; }
    }
}
