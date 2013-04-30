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
using CorrugatedIron.Comms.LoadBalancing;
using CorrugatedIron.Config;
using CorrugatedIron.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public class RiakCluster : RiakEndPoint
    {
        private readonly RoundRobinStrategy _loadBalancer;
        private readonly List<IRiakNode> _nodes;
        private readonly ConcurrentQueue<IRiakNode> _offlineNodes;
        private readonly int _nodePollTime;
        private readonly Timer _nodePollTimer;
        private readonly int _defaultRetryCount;
        private bool _disposing;

        protected override int DefaultRetryCount
        {
            get { return _defaultRetryCount; }
        }

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration, IRiakConnectionFactory connectionFactory)
        {
            _nodePollTime = clusterConfiguration.NodePollTime;
            _nodes = clusterConfiguration.RiakNodes.Select(rn => new RiakNode(rn, connectionFactory)).Cast<IRiakNode>().ToList();
            _loadBalancer = new RoundRobinStrategy();
            _loadBalancer.Initialise(_nodes);
            _offlineNodes = new ConcurrentQueue<IRiakNode>();
            _defaultRetryCount = clusterConfiguration.DefaultRetryCount;
            RetryWaitTime = clusterConfiguration.DefaultRetryWaitTime;

            // node monitor is now asynchronous, just triggered by timer!
            _nodePollTimer = new Timer(state => NodeMonitorCycle(), null, _nodePollTime, Timeout.Infinite);
        }

        /// <summary>
        /// Creates an instance of <see cref="CorrugatedIron.IRiakClient"/> populated from from the configuration section
        /// specified by <paramref name="configSectionName"/>.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section to load the settings from.</param>
        /// <returns>A fully configured <see cref="IRiakEndPoint"/></returns>
        public static IRiakEndPoint FromConfig(string configSectionName)
        {
            return new RiakCluster(RiakClusterConfiguration.LoadFromConfig(configSectionName), new RiakConnectionFactory());
        }

        /// <summary>
        /// Creates an instance of <see cref="CorrugatedIron.IRiakClient"/> populated from from the configuration section
        /// specified by <paramref name="configSectionName"/>.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section to load the settings from.</param>
        /// <param name="configFileName">The full path and name of the config file to load the configuration from.</param>
        /// <returns>A fully configured <see cref="IRiakEndPoint"/></returns>
        public static IRiakEndPoint FromConfig(string configSectionName, string configFileName)
        {
            return new RiakCluster(RiakClusterConfiguration.LoadFromConfig(configSectionName, configFileName), new RiakConnectionFactory());
        }

        protected override Task<TRiakResult> UseConnection<TRiakResult>(Func<IRiakConnection, Task<TRiakResult>> useFun, Func<ResultCode, string, bool, TRiakResult> onError, int retryAttempts)
        {
            if(retryAttempts < 0)
                return TaskResult(onError(ResultCode.NoRetries, "Unable to access a connection on the cluster.", false));
            if (_disposing)
                return TaskResult(onError(ResultCode.ShuttingDown, "System currently shutting down", true));

            // run on node, note that we have to do some harmless but annoying casting
            var node = _loadBalancer.SelectNode() as RiakNode;
            if (node != null)
            {
                Func<IRiakConnection, Task<RiakResult>> cUseFun = (
                    c => useFun(c).ContinueWith((Task<TRiakResult> t) => (RiakResult)t.Result));

                // use harmless casting
                return node.UseConnection(cUseFun)
                    .ContinueWith((Task<RiakResult> finishedTask) => {
                        var result = (TRiakResult)finishedTask.Result;
                        if (!result.IsSuccess)
                        {
                            if (result.ResultCode == ResultCode.NoConnections)
                            {
                                return DelayTask(RetryWaitTime)
                                    .ContinueWith(delayTask => {
                                        return UseConnection(useFun, onError, retryAttempts - 1);
                                    }).Unwrap();
                            }
                            else if (result.ResultCode == ResultCode.CommunicationError)
                            {
                                if (result.NodeOffline)
                                {
                                    DeactivateNode(node);
                                }

                                return DelayTask(RetryWaitTime)
                                    .ContinueWith(delayTask => {
                                        return UseConnection(useFun, onError, retryAttempts - 1);
                                    }).Unwrap();
                            }
                        }

                        // otherwise we'll return the result that we had at this call to make sure that
                        // the correct/initial error is shown
                        return TaskResult(onError(result.ResultCode, result.ErrorMessage, result.NodeOffline));
                    }).Unwrap();
            }

            // can't get node
            return TaskResult(onError(ResultCode.ClusterOffline, "Unable to access functioning Riak node", true));
        }

        public override Task<RiakResult<IEnumerable<TResult>>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, Task<RiakResult<IEnumerable<TResult>>>> useFun, int retryAttempts)
        {
            if(retryAttempts < 0)
                return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.NoRetries, "Unable to access a connection on the cluster.", false));
            if (_disposing)
                return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "System currently shutting down", true));

            // select a node
            var node = _loadBalancer.SelectNode();
            if (node != null)
            {
                return node.UseDelayedConnection(useFun)
                    .ContinueWith((Task<RiakResult<IEnumerable<TResult>>> finishedTask) => {
                        var result = finishedTask.Result;
                        if (!result.IsSuccess)
                        {
                            if (result.ResultCode == ResultCode.NoConnections)
                            {
                                return DelayTask(RetryWaitTime)
                                    .ContinueWith(delayTask => {
                                        return UseDelayedConnection(useFun, retryAttempts - 1);
                                    }).Unwrap();
                            }
                            if (result.ResultCode == ResultCode.CommunicationError)
                            {
                                if (result.NodeOffline)
                                {
                                    DeactivateNode(node);
                                }

                                return DelayTask(RetryWaitTime)
                                    .ContinueWith(delayTask => {
                                        return UseDelayedConnection(useFun, retryAttempts - 1);
                                    }).Unwrap();
                            }
                        }

                        // out of options
                        return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node", true));
                    }).Unwrap();
            }

            // no functioning node
            return TaskResult(RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node", true));
        }

        private void DeactivateNode(IRiakNode node)
        {
            lock (node)
            {
                if (!_offlineNodes.Contains(node))
                {
                    _loadBalancer.RemoveNode(node);
                    _offlineNodes.Enqueue(node);
                }
            }
        }

        // monitor node, started by timer
        private void NodeMonitorCycle()
        {
            if (!_disposing)
            {
                _nodePollTimer.Change(_nodePollTime, Timeout.Infinite);

                // ping all offline nodes
                var deadNodes = new List<IRiakNode>();
                IRiakNode node = null;
                while (_offlineNodes.TryDequeue(out node) && !_disposing)
                {
                    node.UseConnection(c => c.PbcWriteRead(MessageCode.PingReq, MessageCode.PingResp))
                        .ContinueWith((Task<RiakResult> finishedTask) => {
                            if (finishedTask.Result.IsSuccess)
                            {
                                _loadBalancer.AddNode(node);
                            }
                            else
                            {
                                deadNodes.Add(node);
                            }
                        }).Wait();
                }

                if (!_disposing)
                {
                    foreach (var deadNode in deadNodes)
                    {
                        _offlineNodes.Enqueue(deadNode);
                    }
                }
            }
        }

        public override void Dispose()
        {
            _disposing = true;
            _nodes.ForEach(n => n.Dispose());
            _nodePollTimer.Dispose();
        }

        // wrap a task result
        private Task<T> TaskResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        // async timer
        private Task DelayTask(int milliseconds)
        {
            var source = new TaskCompletionSource<object>();

            var timer = null as Timer;
            timer = new Timer(state => {
                source.SetResult(new object());
                timer.Dispose();
                timer = null;
            }, null, milliseconds, Timeout.Infinite);

            return source.Task;
        }
    }
}
