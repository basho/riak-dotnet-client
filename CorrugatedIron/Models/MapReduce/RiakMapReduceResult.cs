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
using System.Reactive.Linq;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Messages;
using System.Collections.Generic;
using System.Linq;

namespace CorrugatedIron.Models.MapReduce
{
    public class RiakMapReduceResult : IRiakMapReduceResult
    {
        private readonly IEnumerable<RiakMapReduceResultPhase> _phaseResults;

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        internal RiakMapReduceResult(IObservable<RpbMapRedResp> response)
        {
            try
            {
                var res = response.ToEnumerable().ToList();

                var phases = res
                    .GroupBy(r => r.phase)
                    .Select(g => new
                    {
                        Phase = g.Key,
                        PhaseResults = g.Select(rr => rr)
                    });

                _phaseResults = phases
                    .OrderBy(p => p.Phase)
                    .Select(p => new RiakMapReduceResultPhase(p.Phase, p.PhaseResults))
                    .ToList();

                IsSuccess = true;
            }
            catch (RiakException riakException)
            {
                IsSuccess = false;
                ErrorMessage = riakException.ErrorMessage;
            }
        }

        public IEnumerable<RiakMapReduceResultPhase> PhaseResults
        {
            get { return _phaseResults; }
        }
    }
}
