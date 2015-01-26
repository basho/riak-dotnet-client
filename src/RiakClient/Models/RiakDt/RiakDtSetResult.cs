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
using System.Linq;
using RiakClient.Extensions;

namespace RiakClient.Models.RiakDt
{
    public class RiakDtSetResult
    {
        public RiakResult<RiakObject> Result { get; private set; }
        public byte[] Context { get; internal set; }
        public List<byte[]> Values { get; internal set; } 

        public RiakDtSetResult(RiakResult<RiakObject> result, 
                               byte[] context = null,
                               List<byte[]> values = null )
        {
            Result = result;

            if (context != null)
                Context = context;

            Values = values ?? new List<byte[]>();
        }

        public ISet<T> GetObjects<T>(DeserializeObject<T> deserializeObject)
        {
            if (deserializeObject == null)
            {
                throw new ArgumentException("deserializeObject must not be null");
            }

            return Values.Select(v => deserializeObject(v)).ToHashSet();
        }
    }
}
