// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public class RiakPhaseInputs
    {
        private readonly List<RiakBucketKeyInput> _bucketKeyInputs;
        private readonly List<RiakBucketKeyArgInput> _bucketKeyArgInputs;
        private readonly RiakBucketInput _bucketInput;

        public RiakPhaseInputs(RiakBucketInput bucketInput)
        {
            _bucketInput = bucketInput;
        }

        public RiakPhaseInputs(List<RiakBucketKeyInput> bucketKeyInputs)
        {
            _bucketKeyInputs = bucketKeyInputs;
        }

        public RiakPhaseInputs(List<RiakBucketKeyArgInput> bucketKeyArgInputs)
        {
            _bucketKeyArgInputs = bucketKeyArgInputs;
        }

        public JsonWriter WriteJson(JsonWriter writer)
        {
            if (_bucketInput != null)
            {
                return _bucketInput.WriteJson(writer);
            }

            writer.WriteStartArray();
            if (_bucketKeyInputs != null)
            {
                _bucketKeyInputs.ForEach(i =>
                    {
                        writer.WriteStartArray();
                        i.WriteJson(writer);
                        writer.WriteEndArray();
                    });
            }
            else
            {
                _bucketKeyArgInputs.ForEach(i =>
                    {
                        writer.WriteStartArray();
                        i.WriteJson(writer);
                        writer.WriteEndArray();
                    });
            }
            writer.WriteEndArray();

            return writer;
        }

        public static implicit operator RiakPhaseInputs(RiakBucketInput input)
        {
            return new RiakPhaseInputs(input);
        }

        public static implicit operator RiakPhaseInputs(List<RiakBucketKeyInput> input)
        {
            return new RiakPhaseInputs(input);
        }

        public static implicit operator RiakPhaseInputs(List<RiakBucketKeyArgInput> input)
        {
            return new RiakPhaseInputs(input);
        }
    }
}
