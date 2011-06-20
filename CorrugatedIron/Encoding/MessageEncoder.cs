// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using System.Net;

namespace CorrugatedIron.Encoding
{
    public interface IMessageEncoder
    {
        void Encode<T>(T message, Stream destination);
        byte[] Encode<T>(T message);
        T Decode<T>(Stream source) where T : new();
        T Decode<T>(byte[] source) where T : new();
    }

    public class MessageEncoder : IMessageEncoder
    {
        private static readonly Dictionary<MessageCode, Type> MessageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> TypeToMessageCodeMap;

        static MessageEncoder()
        {
            MessageCodeToTypeMap = new Dictionary<MessageCode, Type>
            {
                { MessageCode.ErrorResp, typeof(RpbErrorResp) },
                { MessageCode.PingReq, typeof(RpbPingReq) },
                { MessageCode.PingResp, typeof(RpbPingResp) },
                { MessageCode.GetClientIdReq, typeof(RpbGetClientIdReq) },
                { MessageCode.GetClientIdResp, typeof(RpbGetClientIdResp) },
                { MessageCode.SetClientIdReq, typeof(RpbSetClientIdReq) },
                { MessageCode.SetClientIdResp, typeof(RpbSetClientIdResp) },
                { MessageCode.GetServerInfoReq, typeof(RpbGetServerInfoReq) },
                { MessageCode.GetServerInfoResp, typeof(RpbGetServerInfoResp) },
                { MessageCode.GetReq, typeof(RpbGetReq) },
                { MessageCode.GetResp, typeof(RpbGetResp) },
                { MessageCode.PutReq, typeof(RpbPutReq) },
                { MessageCode.PutResp, typeof(RpbPutResp) },
                { MessageCode.DelReq, typeof(RpbDelReq) },
                { MessageCode.DelResp, typeof(RpbDelResp) },
                { MessageCode.ListBucketsReq, typeof(RpbListBucketsReq) },
                { MessageCode.ListBucketsResp, typeof(RpbListBucketsResp) },
                { MessageCode.ListKeysReq, typeof(RpbListKeysReq) },
                { MessageCode.ListKeysResp, typeof(RpbListKeysResp) },
                { MessageCode.GetBucketReq, typeof(RpbGetBucketReq) },
                { MessageCode.GetBucketResp, typeof(RpbGetBucketResp) },
                { MessageCode.SetBucketReq, typeof(RpbSetBucketReq) },
                { MessageCode.SetBucketResp, typeof(RpbSetBucketResp) },
                { MessageCode.MapRedReq, typeof(RpbMapRedReq) },
                { MessageCode.MapRedResp, typeof(RpbMapRedResp) }
            };

            TypeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach (var item in MessageCodeToTypeMap)
            {
                TypeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }

        public void Encode<T>(T message, Stream destination)
        {
            var messageBody = default(byte[]);

            using (var memStream = new MemoryStream())
            {
                Serializer.Serialize(memStream, message);
                messageBody = memStream.GetBuffer();
                Array.Resize(ref messageBody, (int)memStream.Position);
            }

            var messageCode = TypeToMessageCodeMap[typeof(T)];
            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(messageBody.Length + 1));
            destination.Write(size, 0, size.Length);
            destination.WriteByte((byte)messageCode);
            if (messageBody.Length > 0)
            {
                //messageBody.ForEach(b => System.Diagnostics.Debug.Write("{0:X}".Fmt(b)));
                //System.Diagnostics.Debug.WriteLine("");
                destination.Write(messageBody, 0, messageBody.Length);
            }
        }

        public byte[] Encode<T>(T message)
        {
            using (var memStream = new MemoryStream())
            {
                Encode(message, memStream);
                var messageBody = memStream.GetBuffer();
                Array.Resize(ref messageBody, (int)memStream.Position);
                return messageBody;
            }
        }

        public T Decode<T>(Stream source) where T : new()
        {
            var length = new byte[4];
            source.Read(length, 0, length.Length);
            var size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(length, 0));

            var messageCode = (MessageCode)source.ReadByte();
            if (messageCode == MessageCode.ErrorResp)
            {
                var error = DeserializeInstance<RpbErrorResp>(source, size);
                throw new RiakException(error.ErrorCode, error.ErrorMessage.FromRiakString());
            }

#if DEBUG
            // This message code validation is here to make sure that the caller
            // is getting exactly what they expect. This "could" be removed from
            // production code, but it's a good thing to have in here for dev.
            if (MessageCodeToTypeMap[messageCode] != typeof(T))
            {
                throw new InvalidOperationException(string.Format("Attempt to decode message to type '{0}' when received type '{1}'.", typeof(T).Name, MessageCodeToTypeMap[messageCode].Name));
            }
#endif

            if (size <= 1)
            {
                return new T();
            }

            var resultBuffer = new byte[size - 1];
            source.Read(resultBuffer, 0, resultBuffer.Length);

            using (var memStream = new MemoryStream(resultBuffer))
            {
                return Serializer.Deserialize<T>(memStream);
            }
        }

        public T Decode<T>(byte[] source) where T : new()
        {
            using (var memStream = new MemoryStream(source, false))
            {
                return Decode<T>(memStream);
            }
        }

        private static T DeserializeInstance<T>(Stream source, int size)
            where T : new()
        {
            if (size <= 1)
            {
                return new T();
            }

            var resultBuffer = new byte[size - 1];
            source.Read(resultBuffer, 0, resultBuffer.Length);

            using (var memStream = new MemoryStream(resultBuffer))
            {
                return Serializer.Deserialize<T>(memStream);
            }
        }
    }
}
