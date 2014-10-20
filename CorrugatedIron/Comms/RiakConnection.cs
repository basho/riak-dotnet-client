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

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models.Rest;
using CorrugatedIron.Util;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CorrugatedIron.Comms
{
    internal class RiakConnection : IRiakConnection
    {
        static RiakConnection()
        {
            ServicePointManager.ServerCertificateValidationCallback += ServerValidationCallback;
        }

        private Task<TResult> PbcRead<TResult>(RiakPbcSocket socket)
            where TResult : class, new()
        {
            try
            {
                return socket.Read<TResult>();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public async Task<TResult> PbcRead<TResult>(IRiakEndPoint endPoint)
            where TResult : class, new()
        {
            var result = await endPoint.GetSingleResultViaPbc(async socket => await PbcRead<TResult>(socket).ConfigureAwait(false));

            return result;
        }

        private async Task PbcRead(RiakPbcSocket socket, MessageCode expectedMessageCode)
        {
            try
            {
                await socket.Read(expectedMessageCode).ConfigureAwait(false);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public async Task PbcRead(IRiakEndPoint endPoint, MessageCode expectedMessageCode)
        {
            await endPoint.GetSingleResultViaPbc(async socket => await PbcRead(socket, expectedMessageCode).ConfigureAwait(false));
        }

        public IObservable<TResult> PbcRepeatRead<TResult>(IRiakEndPoint endPoint, Func<TResult, bool> repeatRead)
            where TResult : class, new()
        {
            var pbcStreamReadIterator = Observable.Create<TResult>(async observer =>
            {
                await endPoint.GetMultipleResultViaPbc(async socket =>
                {
                    try
                    {
                        TResult result;
                        do
                        {
                            result = await PbcRead<TResult>(socket).ConfigureAwait(false);
                            observer.OnNext(result);
                        } while (repeatRead(result));
                        observer.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        observer.OnError(exception);
                    }
                }).ConfigureAwait(false);
                return Disposable.Empty;
            });

            return pbcStreamReadIterator;
        }

        private async Task PbcWrite<TRequest>(RiakPbcSocket socket, TRequest request)
            where TRequest : class
        {
            try
            {
                await socket.Write(request).ConfigureAwait(false);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public async Task PbcWrite<TRequest>(IRiakEndPoint endPoint, TRequest request)
            where TRequest : class
        {
            await endPoint.GetSingleResultViaPbc(async socket => await PbcWrite(socket, request).ConfigureAwait(false));
        }

        private async Task PbcWrite(RiakPbcSocket socket, MessageCode messageCode)
        {
            try
            {
                await socket.Write(messageCode).ConfigureAwait(false);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect(socket).ConfigureAwait(false).GetAwaiter().GetResult();
                throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public Task PbcWrite(IRiakEndPoint endPoint, MessageCode messageCode)
        {
            return endPoint.GetSingleResultViaPbc(async socket => await PbcWrite(socket, messageCode).ConfigureAwait(false));
        }

        public async Task<TResult> PbcWriteRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request)
            where TRequest : class
            where TResult : class, new()
        {
            var result = await endPoint.GetSingleResultViaPbc(async socket =>
            {
                await PbcWrite(socket, request).ConfigureAwait(false);
                var singleResult = await PbcRead<TResult>(socket).ConfigureAwait(false);
                return singleResult;
            }).ConfigureAwait(false);

            return result;
        }

        public async Task PbcWriteRead<TRequest>(IRiakEndPoint endPoint, TRequest request, MessageCode expectedMessageCode)
            where TRequest : class
        {
            await endPoint.GetSingleResultViaPbc(async socket =>
            {
                await PbcWrite(socket, request).ConfigureAwait(false);
                await PbcRead(socket, expectedMessageCode).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        public async Task<TResult> PbcWriteRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode)
            where TResult : class, new()
        {
            var result = await endPoint.GetSingleResultViaPbc(async socket =>
            {
                await PbcWrite(socket, messageCode).ConfigureAwait(false);
                var result2 = await PbcRead<TResult>(socket).ConfigureAwait(false);
                return result2;
            }).ConfigureAwait(false);

            return result;
        }

        public async Task PbcWriteRead(IRiakEndPoint endPoint, MessageCode messageCode, MessageCode expectedMessageCode)
        {
            await endPoint.GetSingleResultViaPbc(async socket =>
            {
                await PbcWrite(socket, messageCode).ConfigureAwait(false);
                await PbcRead(socket, expectedMessageCode).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        public IObservable<TResult> PbcWriteRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request, Func<TResult, bool> repeatRead)
            where TRequest : class
            where TResult : class, new()
        {
            return PbcWriteStreamReadIterator(endPoint, request, repeatRead);
        }

        public IObservable<TResult> PbcWriteRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode,
            Func<TResult, bool> repeatRead)
            where TResult : class, new()
        {
            return PbcWriteStreamReadIterator(endPoint, messageCode, repeatRead);
        }

        public IObservable<TResult> PbcStreamRead<TResult>(IRiakEndPoint endPoint, Func<TResult, bool> repeatRead)
            where TResult : class, new()
        {
            var pbcStreamReadIterator = Observable.Create<TResult>(async observer =>
            {
                await endPoint.GetMultipleResultViaPbc(async socket =>
                {
                    try
                    {
                        TResult result;
                        do
                        {
                            result = await PbcRead<TResult>(socket).ConfigureAwait(false);
                            observer.OnNext(result);
                        } while (repeatRead(result));
                        observer.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        observer.OnError(exception);
                    }
                }).ConfigureAwait(false);
                return Disposable.Empty;
            });

            return pbcStreamReadIterator;
        }

        public IObservable<TResult> PbcWriteStreamRead<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request,
            Func<TResult, bool> repeatRead)
            where TRequest : class
            where TResult : class, new()
        {
            return PbcWriteStreamReadIterator(endPoint, request, repeatRead);
        }

        public IObservable<TResult> PbcWriteStreamRead<TResult>(IRiakEndPoint endPoint, MessageCode messageCode,
            Func<TResult, bool> repeatRead)
            where TResult : class, new()
        {
            return PbcWriteStreamReadIterator(endPoint, messageCode, repeatRead);
        }

        private IObservable<TResult> PbcWriteStreamReadIterator<TRequest, TResult>(IRiakEndPoint endPoint, TRequest request,
            Func<TResult, bool> repeatRead)
            where TRequest : class
            where TResult : class, new()
        {
            var pbcStreamWriteReadIterator = Observable.Create<TResult>(async observer =>
            {
                await endPoint.GetMultipleResultViaPbc(async socket =>
                {
                    try
                    {
                        await PbcWrite(socket, request).ConfigureAwait(false);

                        TResult result;
                        do
                        {
                            result = await PbcRead<TResult>(socket).ConfigureAwait(false);
                            observer.OnNext(result);
                        } while (repeatRead(result));

                        observer.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        observer.OnError(exception);
                    }
                }).ConfigureAwait(false);
                return Disposable.Empty;
            });

            return pbcStreamWriteReadIterator;
        }

        private IObservable<TResult> PbcWriteStreamReadIterator<TResult>(IRiakEndPoint endPoint, MessageCode messageCode,
            Func<TResult, bool> repeatRead)
            where TResult : class, new()
        {
            var pbcStreamWriteReadIterator = Observable.Create<TResult>(async observer =>
            {
                await endPoint.GetMultipleResultViaPbc(async socket =>
                {
                    try
                    {
                        await PbcWrite(socket, messageCode).ConfigureAwait(false);
                        TResult result = null;
                        do
                        {
                            try
                            {
                                result = await PbcRead<TResult>(socket).ConfigureAwait(false);
                                observer.OnNext(result);
                            }
                            catch (Exception exception)
                            {
                                observer.OnError(exception);
                            }

                        } while (repeatRead(result));
                        observer.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        observer.OnError(exception);
                    }
                }).ConfigureAwait(false);
                return Disposable.Empty;
            });

            return pbcStreamWriteReadIterator;
        }

        public Task<RiakRestResponse> RestRequest(IRiakEndPoint endPoint, RiakRestRequest request)
        {
            return endPoint.GetSingleResultViaRest(async serverUrl =>
            {
                var baseUri = new StringBuilder(serverUrl).Append(request.Uri);
                if (request.QueryParams.Count > 0)
                {
                    baseUri.Append("?");
                    var first = request.QueryParams.First();
                    baseUri.Append(first.Key.UrlEncoded()).Append("=").Append(first.Value.UrlEncoded());
                    request.QueryParams.Skip(1)
                        .ForEach(
                            kv =>
                                baseUri.Append("&")
                                    .Append(kv.Key.UrlEncoded())
                                    .Append("=")
                                    .Append(kv.Value.UrlEncoded()));
                }
                var targetUri = new Uri(baseUri.ToString());

                var req = (HttpWebRequest)WebRequest.Create(targetUri);
                req.KeepAlive = true;
                req.Method = request.Method;
                req.Credentials = CredentialCache.DefaultCredentials;

                if (!string.IsNullOrWhiteSpace(request.ContentType))
                {
                    req.ContentType = request.ContentType;
                }

                if (!request.Cache)
                {
                    req.Headers.Set(RiakConstants.Rest.HttpHeaders.DisableCacheKey,
                        RiakConstants.Rest.HttpHeaders.DisableCacheValue);
                }

                request.Headers.ForEach(h => req.Headers.Set(h.Key, h.Value));

                if (request.Body != null && request.Body.Length > 0)
                {
                    req.ContentLength = request.Body.Length;

                    var writer = await req.GetRequestStreamAsync().ConfigureAwait(false);
                    writer.Write(request.Body, 0, request.Body.Length);
                }
                else
                {
                    req.ContentLength = 0;
                }

                try
                {
                    var response = (HttpWebResponse)await req.GetResponseAsync().ConfigureAwait(false);

                    var result = new RiakRestResponse
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

                    if (result.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new RiakException((uint)ResultCode.InvalidResponse, "Unexpected Status Code: {0} ({1})".Fmt(result.StatusCode, (int)result.StatusCode), false);
                    }

                    return result;
                }
                catch (RiakException ex)
                {
                    throw;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        throw new RiakException((uint)ResultCode.HttpError, ex.Message, false);
                    }

                    throw new RiakException((uint)ResultCode.HttpError, ex.Message, true);
                }
                catch (Exception ex)
                {
                    throw new RiakException((uint)ResultCode.CommunicationError, ex.Message, true);
                }
            });
        }

        private static bool ServerValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private async Task Disconnect(RiakPbcSocket socket)
        {
            if (socket != null)
            {
                await socket.Disconnect().ConfigureAwait(false);
            }
        }
    }
}
