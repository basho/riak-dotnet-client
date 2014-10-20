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

using System.Threading.Tasks;
using CorrugatedIron.Comms;
using System;

namespace CorrugatedIron
{
    public interface IRiakEndPoint : IDisposable
    {
        IRiakClient CreateClient();

        Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun);
        Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun);
        Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun);

        Task GetSingleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun);
        Task<TResult> GetSingleResultViaPbc<TResult>(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task<TResult>> useFun);
        Task GetMultipleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun);

        Task GetSingleResultViaRest(Func<string, Task> useFun);
        Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun);
        Task GetMultipleResultViaRest(Func<string, Task> useFun);
    }
}