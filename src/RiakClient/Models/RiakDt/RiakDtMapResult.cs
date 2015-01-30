// <copyright file="RiakDtMapResult.cs" company="Basho Technologies, Inc.">
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

using System.Collections.Generic;

namespace RiakClient.Models.RiakDt
{
    public class RiakDtMapResult
    {
        public RiakResult<RiakObject> Result { get; private set; }
        public byte[] Context { get; internal set; }
        public List<RiakDtMapEntry> Values { get; internal set; }

        public RiakDtMapResult(RiakResult<RiakObject> result,
                               byte[] context = null,
                               List<RiakDtMapEntry> values = null)
        {
            Result = result;

            if (context != null)
                Context = context;

            Values = values ?? new List<RiakDtMapEntry>();
        }
    }
}
