namespace RiakClient.Models.RiakDt
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// Represents a Riak Counter data type.
    /// </summary>
    [System.Obsolete("RiakDt is deprecated. Please use Commands/CRDT namespace.")]
    public class RiakDtCounter : IRiakDtType<CounterOperation>, IDtOp, IChangeTracking
    {
        private readonly List<CounterOperation> operations = new List<CounterOperation>();
        private readonly byte[] context;
        private long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtCounter"/> class.
        /// </summary>
        public RiakDtCounter()
        {
        }

        // TODO: Deprecate?

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakDtCounter"/> class.
        /// </summary>
        /// <param name="bucket">The bucket name of this counter's object.</param>
        /// <param name="bucketType">The bucket type of this counter's object.</param>
        /// <param name="key">The key of this counter's object.</param>
        /// <param name="response">The response containing the counter's server-side value and context.</param>
        /// <remarks>Not used.</remarks>
        public RiakDtCounter(string bucket, string bucketType, string key, DtFetchResp response)
        {
            Bucket = bucket;
            BucketType = bucketType;
            Key = key;
            value = response.value.counter_value;
            context = response.context;
        }

        /// <summary>
        /// Flag to see if there have been any local changes to the value of this counter.
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// The current local value of this <see cref="RiakDtCounter"/>.
        /// </summary>
        public long Value
        {
            get
            {
                if (IsChanged)
                {
                    return value + operations.Sum(op => op.Value);
                }

                return value;
            }

            internal set
            {
                this.value = value;
            }
        }

        /// <inheritdoc/>
        public string Bucket { get; private set; }

        /// <inheritdoc/>
        public string BucketType { get; private set; }

        /// <inheritdoc/>
        public string Key { get; private set; }

        /// <inheritdoc/>
        public ReadOnlyCollection<CounterOperation> Operations
        {
            get { return operations.AsReadOnly(); }
        }

        /// <summary>
        /// Increment the counter by 1.
        /// </summary>
        /// <param name="counter">The <see cref="RiakDtCounter"/> to increment.</param>
        /// <returns>The incremented <see cref="RiakDtCounter"/>.</returns>
        public static RiakDtCounter operator ++(RiakDtCounter counter)
        {
            return counter.Increment();
        }

        /// <summary>
        /// Increment the counter.
        /// </summary>
        /// <param name="value">The amount (positive or negative) to increment the counter.</param>
        /// <returns>The updated <see cref="RiakDtCounter"/>.</returns>
        /// <remarks>To decrement the counter, use a negative number for <paramref name="value"/>.</remarks>
        public RiakDtCounter Increment(long value = 1)
        {
            operations.Add(new CounterOperation(value));
            IsChanged = true;
            return this;
        }

        // TODO: Deprecate this?

        /// <inheritdoc/>
        public MapEntry ToMapEntry(string fieldName)
        {
            return new MapEntry
                {
                    counter_value = Value,
                    field = new MapField
                        {
                            name = fieldName.ToRiakString(),
                            type = MapField.MapFieldType.COUNTER
                        }
                };
        }

        /// <inheritdoc/>
        public void AcceptChanges()
        {
            value = value + operations.Sum(op => op.Value);
            operations.Clear();
            IsChanged = false;
        }

        /// <summary>
        /// Convert this object to a <see cref="CounterOp"/>.
        /// </summary>
        /// <returns>A newly initialized and configured <see cref="CounterOp"/>.</returns>
        public CounterOp ToCounterOp()
        {
            var sum = operations.Sum(op => op.Value);

            if (sum == 0)
            {
                return null;
            }

            return new CounterOp
                {
                    increment = sum
                };
        }

        /// <inheritdoc/>
        public DtOp ToDtOp()
        {
            var co = ToCounterOp();

            return co == null ? null : new DtOp { counter_op = ToCounterOp() };
        }

        /// <summary>
        /// Compress all operations in a RiakDtCounter into a single DtUpdateReq
        /// </summary>
        /// <param name="options">The RiakDtUpdateOptions</param>
        /// <returns>Returns a valid DtUpdateReq or null.</returns>
        /// <remarks>A null value will be returned when the net of all counter
        /// operations will produce no change to the counter value. That is:
        /// when the sum of all operations is 0, null will be returned. In these
        /// situations, the caller should not submit any changes to Riak. </remarks>
        internal DtUpdateReq ToDtUpdateRequest(RiakDtUpdateOptions options)
        {
            options = options ?? new RiakDtUpdateOptions();

            var request = new DtUpdateReq
                {
                    op = { counter_op = ToCounterOp() }
                };

            /* We shouldn't send any operations in to Riak in this case.
             * This means that whatever calls ToDtUpdateRequest needs to 
             * be aware of possible null values
             */
            if (request.op.counter_op == null || request.op.counter_op.increment == 0)
            {
                return null;
            }

            if (options.IncludeContext && context != null)
            {
                request.context = context;
            }

            options.Populate(request);

            return request;
        }
    }
}
