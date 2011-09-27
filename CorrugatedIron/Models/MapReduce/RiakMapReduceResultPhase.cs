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

using System.Collections.Generic;
using CorrugatedIron.Messages;
using System.Linq;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakMapReduceResultPhase
    {
        public bool Success { get; private set; }
        public uint Phase { get; private set; }
        public List<byte[]> Values { get; private set; }

        internal RiakMapReduceResultPhase(uint phase, IEnumerable<RpbMapRedResp> results)
        {
            Phase = phase;
            Values = results.Select(r => r.Response).ToList(); 
            Success = true;
        }

        internal RiakMapReduceResultPhase()
        {
            Success = false;
        }

        //public T GetObject<T>()
        //{
        //    return JsonConvert.DeserializeObject<T>(Value.FromRiakString());
        //}

        //public dynamic GetObject()
        //{
        //    return JsonConvert.DeserializeObject<dynamic>(Value.FromRiakString());
        //}
    }
}