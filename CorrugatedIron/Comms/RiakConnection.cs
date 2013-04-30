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

using CorrugatedIron.Config;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CorrugatedIron.Comms
{
    public interface IRiakConnection : IDisposable
    {
        bool IsIdle { get; }

        void Disconnect();

        // PBC interface
        Task<RiakResult<TResult>> PbcRead<TResult>()
            where TResult : class, new();

        Task<RiakResult> PbcRead(MessageCode expectedMessageCode);

        Task<RiakResult> PbcWrite<TRequest>(TRequest request)
            where TRequest : class;

        Task<RiakResult> PbcWrite(MessageCode messageCode);

        Task<RiakResult<TResult>> PbcWriteRead<TRequest, TResult>(TRequest request)
            where TRequest : class
            where TResult : class, new();

        Task<RiakResult<TResult>> PbcWriteRead<TResult>(MessageCode messageCode)
            where TResult : class, new();

        Task<RiakResult> PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode)
            where TRequest : class;

        Task<RiakResult> PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode);

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteRead<TResult>(MessageCode messageCode, Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, new();

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteRead<TRequest, TResult>(TRequest request, Func<RiakResult<TResult>, bool> repeatRead)
            where TRequest : class
            where TResult : class, new();

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new();

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteStreamRead<TRequest, TResult>(TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TRequest : class
            where TResult : class, new();

        Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteStreamRead<TResult>(MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new();

        // REST interface
        Task<RiakResult<RiakRestResponse>> RestRequest(RiakRestRequest request);
    }

    // riak connection
    internal class RiakConnection : IRiakConnection
    {
        private readonly string _restRootUrl;
        private readonly RiakPbcSocket _socket;

        public RiakConnection(IRiakNodeConfiguration nodeConfiguration)
        {
            _restRootUrl = @"{0}://{1}:{2}".Fmt(nodeConfiguration.RestScheme, nodeConfiguration.HostAddress, nodeConfiguration.RestPort);
            _socket = new RiakPbcSocket(nodeConfiguration.HostAddress, nodeConfiguration.PbcPort, nodeConfiguration.NetworkReadTimeout,
                                        nodeConfiguration.NetworkWriteTimeout);
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        public Task<RiakResult<TResult>> PbcRead<TResult>() where TResult : class, new()
        {
            return _socket.ReadAsync<TResult>()
                .ContinueWith((Task<TResult> finishedTask) => {
                    if (!finishedTask.IsFaulted)
                        return RiakResult<TResult>.Success(finishedTask.Result);
                    else
                        return GetExceptionResult<TResult>(finishedTask.Exception);
                });
        }

        public Task<RiakResult> PbcRead(MessageCode expectedMessageCode)
        {
            return _socket.ReadAsync(expectedMessageCode)
                .ContinueWith((Task<MessageCode> finishedTask) => {
                    if (!finishedTask.IsFaulted)
                        return RiakResult.Success();
                    else
                        return GetExceptionResult(finishedTask.Exception);
                });
        }

        public Task<RiakResult> PbcWrite<TRequest>(TRequest request) where TRequest : class
        {
            return _socket.WriteAsync<TRequest>(request)
                .ContinueWith((Task finishedTask) => {
                    if (!finishedTask.IsFaulted)
                        return RiakResult.Success();
                    else
                        return GetExceptionResult(finishedTask.Exception);
                });
        }

        public Task<RiakResult> PbcWrite(MessageCode messageCode)
        {
            return _socket.WriteAsync(messageCode)
                .ContinueWith((Task finishedTask) => {
                    if (!finishedTask.IsFaulted)
                        return RiakResult.Success();
                    else
                        return GetExceptionResult(finishedTask.Exception);
                });
        }

        public Task<RiakResult<TResult>> PbcWriteRead<TRequest, TResult>(TRequest request) where TRequest : class where TResult : class, new()
        {
            return PbcWrite(request)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRead<TResult>();
                    }
                    return RiakResult<TResult>.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult<TResult>> PbcWriteRead<TResult>(MessageCode messageCode) where TResult : class, new()
        {
            return PbcWrite(messageCode)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRead<TResult>();
                    }
                    return RiakResult<TResult>.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult> PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode) where TRequest : class
        {
            return PbcWrite(request)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRead(expectedMessageCode);
                    }
                    return RiakResult.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult> PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode)
        {
            return PbcWrite(messageCode)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRead(expectedMessageCode);
                    }
                    return RiakResult.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead) where TResult : class, new()
        {
            var source = new TaskCompletionSource<RiakResult<IEnumerable<RiakResult<TResult>>>>();
            var resultsList = new List<RiakResult<TResult>>();

            // repeat as a continuation
            Action readNext = null;
            readNext = (() => {
                _socket.ReadAsync<TResult>()
                    .ContinueWith((Task<TResult> readTask) => {
                        var result = RiakResult<TResult>.Success(readTask.Result);
                        resultsList.Add(result);

                        if (repeatRead(result))
                        {
                            readNext();
                        }
                        else
                        {
                            source.SetResult(RiakResult<IEnumerable<RiakResult<TResult>>>.Success(resultsList));
                        }
                    });
            });

            // begin reading and completion task
            readNext();
            return source.Task;
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteRead<TResult>(MessageCode messageCode, Func<RiakResult<TResult>, bool> repeatRead) where TResult : class, new()
        {
            return PbcWrite(messageCode)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRepeatRead<TResult>(repeatRead);
                    }
                    return RiakResult<IEnumerable<RiakResult<TResult>>>.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteRead<TRequest, TResult>(TRequest request, Func<RiakResult<TResult>, bool> repeatRead) where TRequest : class where TResult : class, new()
        {
            return PbcWrite<TRequest>(request)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        return PbcRepeatRead<TResult>(repeatRead);
                    }
                    return RiakResult<IEnumerable<RiakResult<TResult>>>.ErrorTask(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                }).Unwrap();
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish) where TResult : class, new()
        {
            var streamer = PbcStreamReadIterator(repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.SuccessTask(streamer);
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteStreamRead<TRequest, TResult>(TRequest request, Func<RiakResult<TResult>, bool> repeatRead, Action onFinish) where TRequest : class where TResult : class, new()
        {
            return PbcWrite<TRequest>(request)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        var streamer = PbcStreamReadIterator(repeatRead, onFinish);
                        return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
                    }
                    return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                });
        }

        public Task<RiakResult<IEnumerable<RiakResult<TResult>>>> PbcWriteStreamRead<TResult>(MessageCode messageCode, Func<RiakResult<TResult>, bool> repeatRead, Action onFinish) where TResult : class, new()
        {
            return PbcWrite(messageCode)
                .ContinueWith((Task<RiakResult> writeTask) => {
                    var writeResult = writeTask.Result;
                    if (writeResult.IsSuccess)
                    {
                        var streamer = PbcStreamReadIterator(repeatRead, onFinish);
                        return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
                    }
                    return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
                });
        }

        public Task<RiakResult<RiakRestResponse>> RestRequest(RiakRestRequest request)
        {
            var baseUri = new StringBuilder(_restRootUrl).Append(request.Uri);
            if(request.QueryParams.Count > 0)
            {
                baseUri.Append("?");
                var first = request.QueryParams.First();
                baseUri.Append(first.Key.UrlEncoded()).Append("=").Append(first.Value.UrlEncoded());
                request.QueryParams.Skip(1).ForEach(kv => baseUri.Append("&").Append(kv.Key.UrlEncoded()).Append("=").Append(kv.Value.UrlEncoded()));
            }
            var targetUri = new Uri(baseUri.ToString());
            
            var req = (HttpWebRequest)WebRequest.Create(targetUri);
            req.KeepAlive = true;
            req.Method = request.Method;
            req.Credentials = CredentialCache.DefaultCredentials;
            
            if(!string.IsNullOrWhiteSpace(request.ContentType))
            {
                req.ContentType = request.ContentType;
            }
            
            if(!request.Cache)
            {
                req.Headers.Set(RiakConstants.Rest.HttpHeaders.DisableCacheKey, RiakConstants.Rest.HttpHeaders.DisableCacheValue);
            }
            
            request.Headers.ForEach(h => req.Headers.Set(h.Key, h.Value));
            
            if(request.Body != null && request.Body.Length > 0)
            {
                req.ContentLength = request.Body.Length;
                using(var writer = req.GetRequestStream())
                {
                    writer.Write(request.Body, 0, request.Body.Length);
                }
            }
            else
            {
                req.ContentLength = 0;
            }

            // process response
            var asyncResult = req.BeginGetResponse(null, null);
            return Task<WebResponse>.Factory.FromAsync(asyncResult, req.EndGetResponse)
                .ContinueWith((Task<WebResponse> responseTask) => {
                    if (!responseTask.IsFaulted)
                    {
                        var response = (HttpWebResponse)responseTask.Result;

                        var result = new RiakRestResponse()
                        {
                            ContentLength = response.ContentLength,
                            ContentType = response.ContentType,
                            StatusCode = response.StatusCode,
                            Headers = response.Headers.AllKeys.ToDictionary(k => k, k => response.Headers[k]),
                            ContentEncoding = !string.IsNullOrWhiteSpace(response.ContentEncoding)
                                            ? Encoding.GetEncoding(response.ContentEncoding)
                                            : Encoding.Default
                        };
                        
                        if (response.ContentLength > 0)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                {
                                    using (var reader = new StreamReader(responseStream, result.ContentEncoding))
                                    {
                                        result.Body = reader.ReadToEnd();
                                    }
                                }
                            }
                        }
                        
                        return RiakResult<RiakRestResponse>.Success(result);
                    }
                    else
                    {
                        var exceptions = responseTask.Exception.Flatten().InnerExceptions;
                        var rEx = exceptions.OfType<RiakException>().FirstOrDefault();
                        var wEx = exceptions.OfType<WebException>().FirstOrDefault();

                        // process exceptions
                        if (rEx != null)
                        {
                            return RiakResult<RiakRestResponse>.Error(ResultCode.CommunicationError, rEx.Message, rEx.NodeOffline);
                        }
                        else if (wEx != null)
                        {
                            if (wEx.Status == WebExceptionStatus.ProtocolError)
                            {
                                return RiakResult<RiakRestResponse>.Error(ResultCode.HttpError, wEx.Message, false);
                            }
                            
                            return RiakResult<RiakRestResponse>.Error(ResultCode.HttpError, wEx.Message, true);
                        }
                        else
                        {
                            var ex = exceptions.FirstOrDefault();
                            return RiakResult<RiakRestResponse>.Error(ResultCode.CommunicationError, ex.Message, true);
                        }
                    }
                });
        }

        public bool IsIdle
        {
            get { return _socket.IsConnected; }
        }

        public void Dispose()
        {
            Disconnect();
            _socket.Dispose();
        }

        // read as a stream - will add bits and pieces as they become available
        private IEnumerable<RiakResult<TResult>> PbcStreamReadIterator<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, new()
        {
            RiakResult<TResult> result;
            
            do
            {

                // block wait for result
                var task = PbcRead<TResult>();
                task.Wait();
                result = task.Result;

                // sync behaviour
                if(!result.IsSuccess)
                {
                    break;
                }
                else
                {
                    yield return result;
                }
            } while(repeatRead(result));
            
            // clean up first..
            onFinish();
            
            // then return the failure to the client to indicate failure
            yield return result;
        }

        // tidy up messy exceptions
        private RiakResult GetExceptionResult(AggregateException aggregateException)
        {
            return GetExceptionResult<object>(aggregateException);
        }

        // tidy up messy exceptions
        private RiakResult<T> GetExceptionResult<T>(AggregateException aggregateException)
        {
            var exceptions = aggregateException.Flatten().InnerExceptions;
            var rEx = exceptions.OfType<RiakException>().FirstOrDefault();
            var fEx = exceptions.FirstOrDefault();
            if (rEx != null)
            {
                if (rEx.NodeOffline)
                {
                    Disconnect();
                }
                return RiakResult<T>.Error(ResultCode.CommunicationError, rEx.Message, rEx.NodeOffline);
            }
            else
            {
                Disconnect();
                return RiakResult<T>.Error(ResultCode.CommunicationError, fEx.Message, true);
            }
        }
    }
}
