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

using CorrugatedIron.Comms;
using System;
using System.Collections.Generic;

namespace CorrugatedIron
{
    public interface IRiakEndPoint : IDisposable
    {
        int RetryWaitTime { get; set; }

        IRiakClient CreateClient();

        [Obsolete("Clients no longer need a seed value, use CreateClient() instead. This will be removed in CorrugatedIron 1.5")]
        IRiakClient CreateClient(string seed);

        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts);

        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }
}