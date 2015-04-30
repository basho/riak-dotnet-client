// <copyright file="IRiakConnection.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Comms
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Messages;

    /// <summary>
    /// The public interface for connections to Riak.
    /// </summary>
    public interface IRiakConnection : IDisposable
    {
        void Disconnect();

        RiakResult<TResult> PbcRead<TResult>()
            where TResult : class, new();

        RiakResult PbcRead(MessageCode expectedMessageCode);

        RiakResult PbcWrite<TRequest>(TRequest request)
            where TRequest : class;

        RiakResult PbcWrite(MessageCode messageCode);

        RiakResult<TResult> PbcWriteRead<TRequest, TResult>(TRequest request)
            where TRequest : class
            where TResult : class, new();

        RiakResult<TResult> PbcWriteRead<TResult>(MessageCode messageCode)
            where TResult : class, new();

        RiakResult PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode)
            where TRequest : class;

        RiakResult PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode);

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TResult>(MessageCode messageCode, Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TRequest, TResult>(TRequest request, Func<RiakResult<TResult>, bool> repeatRead)
            where TRequest : class
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class
            where TResult : class, new();

        RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, new();

        RiakResult Execute(IRiakCommand command);
    }
}