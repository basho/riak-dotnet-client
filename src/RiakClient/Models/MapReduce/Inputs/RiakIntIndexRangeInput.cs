// <copyright file="RiakIntIndexRangeInput.cs" company="Basho Technologies, Inc.">
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


using System;
using System.Numerics;
using Newtonsoft.Json;

namespace RiakClient.Models.MapReduce.Inputs
{
    public class RiakIntIndexRangeInput : RiakIndexInput
    {
        [Obsolete("Use the constructor that accepts a RiakIndexId instead. This will be revoved in the next version.")]
        public RiakIntIndexRangeInput(string bucket, string index, BigInteger start, BigInteger end)
            : this(new RiakIndexId(bucket, index), start, end)
        {
        }

        public RiakIntIndexRangeInput(RiakIndexId indexId, BigInteger start, BigInteger end)
            : base(indexId.ToIntIndexId())
        {
            Start = start;
            End = end;
        }

        public BigInteger Start { get; set; }
        public BigInteger End { get; set; }

        public override JsonWriter WriteJson(JsonWriter writer)
        {
            WriteIndexHeaderJson(writer);

            writer.WritePropertyName("start");
            writer.WriteValue(Start.ToString());

            writer.WritePropertyName("end");
            writer.WriteValue(End.ToString());

            writer.WriteEndObject();

            return writer;
        }
    }
}
