// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.RiakDt
{
    public class CounterOperation
    {
        public long Value { get; private set; }

        public CounterOperation(long value)
        {
            Value = value;
        }

        public DtOp ToDtOp()
        {
            return new DtOp
                {
                    counter_op = new CounterOp
                        {
                            increment = Value
                        }
                };
        }
    }

    public class Counter : IRiakDtType<CounterOperation>, IChangeTracking 
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
            private set { _value = value; }
        }
        public string Bucket { get; private set; }
        public string BucketType { get; private set; }
        public string Key { get; private set; }

        private readonly List<CounterOperation> _operations = new List<CounterOperation>();

        public Counter(string bucket, string bucketType, string key, DtFetchResp response)
        {
            Bucket = bucket;
            BucketType = bucketType;
            Key = key;
            Value = response.counter_value;
            _context = response.context;
        }
        
        public Counter Increment(long value = 1)
        {
            _operations.Add(new CounterOperation(value));
            IsChanged = true;
            return this;
        }

        public ReadOnlyCollection<CounterOperation> Operations
        {
            get { return _operations.AsReadOnly(); }
        }

        public static Counter operator ++(Counter counter)
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

        public List<DtUpdateReq> ToUpdateRequestList(RiakDtUpdateOptions options)
        {
            options = options ?? new RiakDtUpdateOptions();

            return
                _operations.Select(o =>
                    {
                        var req = new DtUpdateReq()
                            {
                                bucket = Bucket.ToRiakString(),
                                type = BucketType.ToRiakString(),
                                key = Key.ToRiakString(),
                                op = o.ToDtOp()
                            };

                        if (options.IncludeContext)
                            req.context = _context;

                        options.Populate(req);

                        return req;
                    }).ToList();
        }

        private List<DtOp> ToCounterOpList()
        {
            return _operations.Select(op => op.ToDtOp()).ToList();
        }
    }



    public interface IRiakDtType<T> 
    {
        string Bucket { get; }
        string BucketType { get; }
        string Key { get; }
        ReadOnlyCollection<T> Operations { get; }
        MapEntry ToMapEntry(string fieldName);
    }
}
