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

using CorrugatedIron.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using System.Net;

namespace CorrugatedIron.Encoding
{
    public class MessageEncoder : IMessageEncoder
    {
        private static readonly Dictionary<MessageCode, Type> _messageCodeToTypeMap;
        private static readonly Dictionary<Type, MessageCode> _typeToMessageCodeMap;

        static MessageEncoder()
        {
            _messageCodeToTypeMap = new Dictionary<MessageCode, Type>
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

            _typeToMessageCodeMap = new Dictionary<Type, MessageCode>();

            foreach (var item in _messageCodeToTypeMap)
            {
                _typeToMessageCodeMap.Add(item.Value, item.Key);
            }
        }

        public void Encode<T>(T message, Stream destination)
        {
            var messageBody = default(byte[]);

            using (var memStream = new MemoryStream())
            {
                Serializer.Serialize(memStream, message);
                // TODO: make sure this does what we would expect it to do
                // or do some other funky bits to get the sizes right.
                messageBody = memStream.GetBuffer();
                Array.Resize(ref messageBody, (int)memStream.Position);
            }

            var messageCode = _typeToMessageCodeMap[typeof(T)];
            var size = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(messageBody.Length + 1));
            destination.Write(size, 0, size.Length);
            destination.WriteByte((byte)messageCode);
            if (messageBody.Length > 0)
            {
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
            var sizeBuffer = new byte[4];
            source.Read(sizeBuffer, 0, 4);

            var size = default(int);
            if (Serializer.TryReadLengthPrefix(source, PrefixStyle.Fixed32, out size))
            {
                if (size <= 1)
                {
                    return new T();
                }

                var messageCode = (MessageCode)source.ReadByte();
                if (_messageCodeToTypeMap[messageCode] != typeof(T))
                {
                    throw new InvalidOperationException(string.Format("Attempt to decode message to type '{0}' when received type '{1}'.", typeof(T).Name, _messageCodeToTypeMap[messageCode].Name));
                }

                return Serializer.Deserialize<T>(source);
            }

            throw new Exception("Unable to read size from source stream.");
        }

        public T Decode<T>(byte[] source) where T : new()
        {
            using (var memStream = new MemoryStream(source, false))
            {
                return Decode<T>(memStream);
            }
        }
    }
}
