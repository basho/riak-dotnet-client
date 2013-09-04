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
using System.Numerics;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.RiakDt
{
    public class CounterOp
    {
        public long Value { get; private set; }

        public CounterOp(long value)
        {
            Value = value;
        }
    }

    public class Counter : IRiakDtType<CounterOp>, IChangeTracking 
    {
        public bool IsChanged { get; private set; }
        private long _value;
        private byte[] _context;
        private DtFetchResp.DataType _dataType = DtFetchResp.DataType.COUNTER;

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

        private readonly List<CounterOp> _operations = new List<CounterOp>();

        public Counter(DtFetchResp response)
        {
            Value = response.counter_value;
            _context = response.context;
        }
        
        public Counter Increment(long value = 1)
        {
            _operations.Add(new CounterOp(value));
            IsChanged = true;
            return this;
        }

        public ReadOnlyCollection<CounterOp> Operations
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
                    field = new MapField()
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

        
    }

    public interface IRiakDtType<T> 
    {
        ReadOnlyCollection<T> Operations { get; }
        MapEntry ToMapEntry(string fieldName);
    }
}
