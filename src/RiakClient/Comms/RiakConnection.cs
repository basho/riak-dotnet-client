// <copyright file="RiakConnection.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Config;
    using Exceptions;
    using Extensions;
    using Messages;
    using Models.Rest;
    using Util;

    internal class RiakConnection : IRiakConnection
    {
        private readonly string restRootUrl;
        private readonly RiakPbcSocket socket;

        public RiakConnection(IRiakNodeConfiguration nodeConfiguration, IRiakAuthenticationConfiguration authConfiguration)
        {
            restRootUrl = string.Format(@"{0}://{1}:{2}", nodeConfiguration.RestScheme, nodeConfiguration.HostAddress, nodeConfiguration.RestPort);

            socket = new RiakPbcSocket(nodeConfiguration, authConfiguration);
        }

        public RiakResult<TResult> PbcRead<TResult>()
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            try
            {
                var result = socket.Read<TResult>();
                return RiakResult<TResult>.Success(result);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                if (ex.Message.Contains("Bucket cannot be zero-length")
                    || ex.Message.Contains("Key cannot be zero-length"))
                {
                    return RiakResult<TResult>.Error(ResultCode.InvalidRequest, ex.Message, ex.NodeOffline);
                }

                return RiakResult<TResult>.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult<TResult>.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public RiakResult PbcRead(MessageCode expectedMessageCode)
        {
            try
            {
                socket.Read(expectedMessageCode);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcRepeatRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var results = new List<RiakResult<TResult>>();
            try
            {
                RiakResult<TResult> result;

                do
                {
                    result = RiakResult<TResult>.Success(socket.Read<TResult>());
                    results.Add(result);
                }
                while (repeatRead(result));

                return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(results);
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public RiakResult PbcWrite<TRequest>(TRequest request)
            where TRequest : class, ProtoBuf.IExtensible
        {
            try
            {
                socket.Write(request);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public RiakResult PbcWrite(MessageCode messageCode)
        {
            try
            {
                socket.Write(messageCode);
                return RiakResult.Success();
            }
            catch (RiakException ex)
            {
                if (ex.NodeOffline)
                {
                    Disconnect();
                }

                // TODO: we really should preserve the Exception object
                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (Exception ex)
            {
                Disconnect();
                return RiakResult.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public RiakResult<TResult> PbcWriteRead<TRequest, TResult>(TRequest request)
            where TRequest : class, ProtoBuf.IExtensible
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(request);

            if (writeResult.IsSuccess)
            {
                return PbcRead<TResult>();
            }

            return RiakResult<TResult>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult PbcWriteRead<TRequest>(TRequest request, MessageCode expectedMessageCode)
            where TRequest : class, ProtoBuf.IExtensible
        {
            var writeResult = PbcWrite(request);

            if (writeResult.IsSuccess)
            {
                return PbcRead(expectedMessageCode);
            }

            return RiakResult.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult<TResult> PbcWriteRead<TResult>(MessageCode messageCode)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcRead<TResult>();
            }

            return RiakResult<TResult>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult PbcWriteRead(MessageCode messageCode, MessageCode expectedMessageCode)
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcRead(expectedMessageCode);
            }

            return RiakResult.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead)
            where TRequest : class, ProtoBuf.IExtensible
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(request);

            if (writeResult.IsSuccess)
            {
                return PbcRepeatRead(repeatRead);
            }

            return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcRepeatRead(repeatRead);
            }

            return RiakResult<IEnumerable<RiakResult<TResult>>>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcStreamRead<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var streamer = PbcStreamReadIterator(repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class, ProtoBuf.IExtensible
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var streamer = PbcWriteStreamReadIterator(request, repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult<IEnumerable<RiakResult<TResult>>> PbcWriteStreamRead<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var streamer = PbcWriteStreamReadIterator(messageCode, repeatRead, onFinish);
            return RiakResult<IEnumerable<RiakResult<TResult>>>.Success(streamer);
        }

        public RiakResult<RiakRestResponse> RestRequest(RiakRestRequest restRequest)
        {
            var baseUri = new StringBuilder(restRootUrl).Append(restRequest.Uri);

            if (restRequest.QueryParams.Count > 0)
            {
                baseUri.Append("?");
                var first = restRequest.QueryParams.First();
                baseUri.Append(first.Key.UrlEncoded()).Append("=").Append(first.Value.UrlEncoded());
                foreach (var queryParam in restRequest.QueryParams.Skip(1))
                {
                    baseUri.Append("&").Append(queryParam.Key.UrlEncoded()).Append("=").Append(queryParam.Value.UrlEncoded());
                }
            }

            var targetUri = new Uri(baseUri.ToString());

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUri);
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Method = restRequest.Method;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;

            if (!string.IsNullOrWhiteSpace(restRequest.ContentType))
            {
                httpWebRequest.ContentType = restRequest.ContentType;
            }

            if (!restRequest.Cache)
            {
                httpWebRequest.Headers.Set(RiakConstants.Rest.HttpHeaders.DisableCacheKey, RiakConstants.Rest.HttpHeaders.DisableCacheValue);
            }

            foreach (KeyValuePair<string, string> restRequestHeader in restRequest.Headers)
            {
                httpWebRequest.Headers.Set(restRequestHeader.Key, restRequestHeader.Value);
            }

            if (!EnumerableUtil.IsNullOrEmpty(restRequest.Body))
            {
                httpWebRequest.ContentLength = restRequest.Body.Length;
                using (var writer = httpWebRequest.GetRequestStream())
                {
                    writer.Write(restRequest.Body, 0, restRequest.Body.Length);
                }
            }
            else
            {
                httpWebRequest.ContentLength = 0;
            }

            try
            {
                var response = (HttpWebResponse)httpWebRequest.GetResponse();

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

                return RiakResult<RiakRestResponse>.Success(result);
            }
            catch (RiakException ex)
            {
                return RiakResult<RiakRestResponse>.Error(ResultCode.CommunicationError, ex.Message, ex.NodeOffline);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return RiakResult<RiakRestResponse>.Error(ResultCode.HttpError, ex.Message, false);
                }

                return RiakResult<RiakRestResponse>.Error(ResultCode.HttpError, ex.Message, true);
            }
            catch (Exception ex)
            {
                return RiakResult<RiakRestResponse>.Error(ResultCode.CommunicationError, ex.Message, true);
            }
        }

        public void Dispose()
        {
            socket.Dispose();
            Disconnect();
        }

        public void Disconnect()
        {
            socket.Disconnect();
        }

        private IEnumerable<RiakResult<TResult>> PbcWriteStreamReadIterator<TResult>(
            MessageCode messageCode,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(messageCode);

            if (writeResult.IsSuccess)
            {
                return PbcStreamReadIterator(repeatRead, onFinish);
            }

            onFinish();

            return new[] { RiakResult<TResult>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline) };
        }

        private IEnumerable<RiakResult<TResult>> PbcStreamReadIterator<TResult>(Func<RiakResult<TResult>, bool> repeatRead, Action onFinish)
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            RiakResult<TResult> result;

            do
            {
                result = PbcRead<TResult>();

                if (!result.IsSuccess)
                {
                    break;
                }

                yield return result;
            }
            while (repeatRead(result));

            // clean up first..
            onFinish();

            // then return the failure to the client to indicate failure
            yield return result;
        }

        private IEnumerable<RiakResult<TResult>> PbcWriteStreamReadIterator<TRequest, TResult>(
            TRequest request,
            Func<RiakResult<TResult>, bool> repeatRead,
            Action onFinish)
            where TRequest : class, ProtoBuf.IExtensible
            where TResult : class, ProtoBuf.IExtensible, new()
        {
            var writeResult = PbcWrite(request);

            if (writeResult.IsSuccess)
            {
                return PbcStreamReadIterator(repeatRead, onFinish);
            }

            onFinish();

            return new[] { RiakResult<TResult>.Error(writeResult.ResultCode, writeResult.ErrorMessage, writeResult.NodeOffline) };
        }
    }
}
