// <copyright file="CounterOperation.cs" company="Basho Technologies, Inc.">
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
    using Messages;

    public class CounterOperation : IDtOp
    {
        private readonly long value;

        public CounterOperation(long value)
        {
            this.value = value;
        }

        public long Value
        {
            get { return value; }
        }

        public DtOp ToDtOp()
        {
            return new DtOp
                {
                    counter_op = new CounterOp { increment = value }
                };
        }
    }
}
