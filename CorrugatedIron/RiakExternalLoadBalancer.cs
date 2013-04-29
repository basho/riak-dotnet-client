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
using CorrugatedIron.Config;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public class RiakExternalLoadBalancer : RiakEndPoint
    {
        private readonly IRiakExternalLoadBalancerConfiguration _lbConfiguration;
        private readonly RiakNode _node;
        private bool _disposing;

        public RiakExternalLoadBalancer(IRiakExternalLoadBalancerConfiguration lbConfiguration, IRiakConnectionFactory connectionFactory)
        {
            _lbConfiguration = lbConfiguration;
            _node = new RiakNode(_lbConfiguration.Target, connectionFactory);
        }

        public static IRiakEndPoint FromConfig(string configSectionName)
        {
            return new RiakExternalLoadBalancer(RiakExternalLoadBalancerConfiguration.LoadFromConfig(configSectionName), new RiakConnectionFactory());
        }

        public static IRiakEndPoint FromConfig(string configSectionName, string configFileName)
        {
            return new RiakExternalLoadBalancer(RiakExternalLoadBalancerConfiguration.LoadFromConfig(configSectionName, configFileName), new RiakConnectionFactory());
        }

        protected override int DefaultRetryCount
        {
            get { return _lbConfiguration.DefaultRetryCount; }
        }

        protected override Task<TRiakResult> UseConnection<TRiakResult>(Func<IRiakConnection, Task<TRiakResult>> useFun, Func<ResultCode, string, bool, TRiakResult> onError, int retryAttempts)
        {
            if(retryAttempts < 0)
            {
                return TaskResult(onError(ResultCode.NoRetries, "Unable to access a connection on the cluster.", true));
            }
            if(_disposing)
            {
                return TaskResult(onError(ResultCode.ShuttingDown, "System currently shutting down", true));
            }

            // get result (note the harmless but annoying casting)
            var node = _node;
            if(node != null)
            {
                Func<IRiakConnection, Task<RiakResult>> cUseFun = (
                    c => useFun(c).ContinueWith((Task<TRiakResult> t) => (RiakResult)t.Result));

                // use harmess casting
                return node.UseConnection(cUseFun)
                    .ContinueWith((Task<RiakResult> finishedTask) => {
                        var result = (TRiakResult)finishedTask.Result;
                        if(!result.IsSuccess)
                        {
                            return TaskDelay(RetryWaitTime)
                                .ContinueWith(finishedDelayTask => {
                                    return UseConnection(useFun, onError, retryAttempts - 1);
                                }).Unwrap();
                        }
                        return TaskResult(result);
                    }).Unwrap();
            }

            // no functioning node
            return TaskResult(onError(ResultCode.ClusterOffline, "Unable to access functioning Riak node", true));
        }

        public override Task<RiakResult<IEnumerable<TResult>>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, Task<RiakResult<IEnumerable<TResult>>>> useFun, int retryAttempts)
        {
            if(retryAttempts < 0)
            {
                return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.NoRetries, "Unable to access a connection on the cluster.", true));
            }
            if(_disposing)
            {
                return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "System currently shutting down", true));
            }

            var node = _node;
            if(node != null)
            {
                return node.UseDelayedConnection(useFun)
                    .ContinueWith((Task<RiakResult<IEnumerable<TResult>>> finishedTask) => {
                        var result = finishedTask.Result;
                        if(!result.IsSuccess)
                        {
                            return TaskDelay(RetryWaitTime).ContinueWith(finishedDelayTask => {
                                return UseDelayedConnection(useFun, retryAttempts - 1);
                            }).Unwrap();
                        }
                        return TaskResult(result);
                    }).Unwrap();
            }
            return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node", true));
        }

        public override void Dispose()
        {
            _disposing = true;

            _node.Dispose();
        }

        // wrap a task result
        private Task<T> TaskResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        // create a delay task
        private Task TaskDelay(int milliseconds)
        {
            var source = new TaskCompletionSource<object>();

            // setup timer
            var timer = null as Timer;
            timer = new Timer(dummyState => {
                source.SetResult(new object());
                timer.Dispose();
                timer = null;
            }, null, milliseconds, Timeout.Infinite);

            // give back the task
            return source.Task;
        }
    }
}