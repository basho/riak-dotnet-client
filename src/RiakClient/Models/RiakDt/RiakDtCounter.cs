// <copyright file="RiakDtCounter.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Models.RiakDt
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Extensions;
    using Messages;

    public class RiakDtCounter : IRiakDtType<CounterOperation>, IDtOp, IChangeTracking 
    {
        public bool IsChanged { get; private set; }
        private long _value;
        private readonly byte[] _context;

        public long Value 
        { 
            get
            {
                if (IsChanged)
                {
                    return _value + _operations.Sum(op => op.Value);
                }

                return _value;
            }
            internal set { _value = value; }
        }
        public string Bucket { get; private set; }
        public string BucketType { get; private set; }
        public string Key { get; private set; }

        private readonly List<CounterOperation> _operations = new List<CounterOperation>();

        public RiakDtCounter()
        {
            
        }

        public RiakDtCounter(string bucket, string bucketType, string key, DtFetchResp response)
        {
            Bucket = bucket;
            BucketType = bucketType;
            Key = key;
            Value = response.value.counter_value;
            _context = response.context;
        }
        
        public RiakDtCounter Increment(long value = 1)
        {
            _operations.Add(new CounterOperation(value));
            IsChanged = true;
            return this;
        }

        public ReadOnlyCollection<CounterOperation> Operations
        {
            get { return _operations.AsReadOnly(); }
        }

        public static RiakDtCounter operator ++(RiakDtCounter counter)
        {
            return counter.Increment();
        }

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

        public void AcceptChanges()
        {
            _value = _value + _operations.Sum(op => op.Value);
            _operations.Clear();
            IsChanged = false;
        }

        public CounterOp ToCounterOp()
        {
            var sum = _operations.Sum(op => op.Value);

            if (sum == 0)
                return null;

            return new CounterOp
                {
                    increment = sum
                };
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
        public DtUpdateReq ToDtUpdateRequest(RiakDtUpdateOptions options)
        {
            options = options ?? new RiakDtUpdateOptions();

            var request = new DtUpdateReq
                {
                    op = {counter_op = ToCounterOp()}
                };

            /* We shouldn't send any operations in to Riak in this case.
             * This means that whatever calls ToDtUpdateRequest needs to 
             * be aware of possible null values
             */
            if (request.op.counter_op == null || request.op.counter_op.increment == 0)
                return null;

            if (options.IncludeContext && _context != null)
                request.context = _context;

            options.Populate(request);

            return request;
        }

        public DtOp ToDtOp()
        {
            var co = ToCounterOp();

            return co == null ? null : new DtOp { counter_op = ToCounterOp() };
        }
    }
}
