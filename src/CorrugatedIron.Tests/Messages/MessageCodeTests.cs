// Copyright (c) 2015 Basho Technologies, Inc.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorrugatedIron.Messages;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Messages
{
    [TestFixture]
    public class MessageCodeTests
    {
        private static readonly char[] csv_split = new char[] { ',' };

        [Test]
        public void AllMessageCodesInMessageCodeEnum()
        {
            var messageMapByName = new Dictionary<string, ushort>();

            var currentDir = Environment.CurrentDirectory;
            string riak_pb_messages_file =
                Path.Combine(currentDir, "..", "..", "..", "riak_pb", "src", "riak_pb_messages.csv");

            string[] msg_csv = File.ReadAllLines(riak_pb_messages_file);
            foreach (string line in msg_csv)
            {
                string[] parts = line.Split(csv_split);
                ushort messageCode = UInt16.Parse(parts[0]);
                string messageName = parts[1];

                messageMapByName.Add(messageName, messageCode);
            }

            var messageCodeEnumNames = Enum.GetNames(typeof(MessageCode));
            foreach (string name in messageMapByName.Keys)
            {
                Assert.True(messageCodeEnumNames.Contains(name),
                    String.Format("CSV contains name '{0}' but enum does NOT", name));

                ushort messageCodeValue = messageMapByName[name];

                MessageCode parsedMessageCode;
                Assert.True(Enum.TryParse(messageCodeValue.ToString(), out parsedMessageCode),
                    String.Format("Can't parse message code '{0}' with value '{1}' as Message Code enum",
                        name, messageCodeValue));

                bool condition =
                    parsedMessageCode.Equals(MessageCode.RpbPingReq) || // NB: Does not have a protobuf message class
                    parsedMessageCode.Equals(MessageCode.RpbPingResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbGetClientIdReq) || // NB: unused
                    parsedMessageCode.Equals(MessageCode.RpbGetClientIdResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbSetClientIdReq) ||
                    parsedMessageCode.Equals(MessageCode.RpbSetClientIdResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbGetServerInfoReq) ||
                    parsedMessageCode.Equals(MessageCode.RpbDelResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbSetBucketResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbResetBucketResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbAuthResp) ||
                    parsedMessageCode.Equals(MessageCode.RpbStartTls) ||
                    MessageCodeTypeMapBuilder.Contains(parsedMessageCode);
                Assert.True(condition, String.Format("MessageCodeTypeMapBuilder does NOT contain '{0}'", parsedMessageCode));

            }

            foreach (string name in messageCodeEnumNames)
            {
                Assert.True(messageMapByName.ContainsKey(name),
                    String.Format("MessageCode enum contains name '{0}' but CSV does NOT", name));
            }
        }
    }
}
