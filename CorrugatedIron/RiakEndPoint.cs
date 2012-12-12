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
    public abstract class RiakEndPoint : IRiakEndPoint
    {
        public int RetryWaitTime { get; set; }
        protected abstract int DefaultRetryCount { get; }

        /// <summary>
        /// [Obsolete] Creates a new instance of <see cref="CorrugatedIron.RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        /// <param name='seed'>
        /// An optional seed to generate the Client Id for the <see cref="CorrugatedIron.RiakClient"/>. Having a unique Client Id is important for
        /// generating good vclocks. For more information about the importance of vector clocks, refer to http://wiki.basho.com/Vector-Clocks.html
        /// </param>
        [Obsolete("Clients no longer need a seed value, use CreateClient() instead")]
        public IRiakClient CreateClient(string seed)
        {
            return new RiakClient(this, seed) { RetryCount = DefaultRetryCount };
        }

        /// <summary>
        /// Creates a new instance of <see cref="CorrugatedIron.RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        public IRiakClient CreateClient()
        {
            return new RiakClient(this) { RetryCount = DefaultRetryCount };
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult.Error, retryAttempts);
        }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult<TResult>.Error, retryAttempts);
        }

        protected abstract TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, bool, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult;

        public abstract RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;

        public abstract void Dispose();
    }
}