namespace RiakClient.Models.RiakDt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an operation result for a Riak Set data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class RiakDtSetResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtSetResult"/> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="context">The current context, if one was returned with the operation result.</param>
        /// <param name="values">The current collection of <see cref="RiakDtMapEntry"/>s, if they were returned with the operation result.</param>
        public RiakDtSetResult(
            RiakResult<RiakObject> result,
            byte[] context = null,
            List<byte[]> values = null)
        {
            Result = result;

            if (context != null)
            {
                Context = context;
            }

            Values = values ?? new List<byte[]>();
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
        /// The updated collection of raw set values.
        /// </summary>
        public List<byte[]> Values { get; internal set; }

        /// <summary>
        /// Deserialize the <see cref="Values"/> collection into a 
        /// </summary>
        /// <typeparam name="T">The set members target type.</typeparam>
        /// <param name="deserializeObject">
        /// A delegate to handle deserialization of an byte[] serialized object to it's original type.
        /// </param>
        /// <returns>
        /// A newly initialized <see cref="ISet{T}"/> collection, 
        /// populated with the deserialized objects from <see cref="Values"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="deserializeObject"/> parameter must not be null.</exception>
        public ISet<T> GetObjects<T>(DeserializeObject<T> deserializeObject)
        {
            if (deserializeObject == null)
            {
                throw new ArgumentException("The deserializeObject delegate parameter must not be null.");
            }

            return new HashSet<T>(Values.Select(v => deserializeObject(v)));
        }
    }
}
