// <copyright file="DataTypeTestsBase.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live.DataTypes
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using RiakClient.Models;

    [SkipMono]
    public class DataTypeTestsBase : LiveRiakConnectionTestBase
    {
        protected const string Bucket = "riak_dt_bucket";
        protected readonly DeserializeObject<string> Deserializer = (b, type) => Encoding.UTF8.GetString(b);
        protected readonly SerializeObjectToByteArray<string> Serializer = s => Encoding.UTF8.GetBytes(s);

        //TODO: use CallerMemberNameAttribute when we move to .Net 4.5
        protected string GetRandomKey(string memberName = "")
        {
            if (string.IsNullOrEmpty(memberName))
            {
                var frame = new StackFrame(1);
                memberName = frame.GetMethod().Name;
            }
            var key = string.Format("{0}_{1}", memberName, Random.Next());
            Console.WriteLine("Using {0} for {1}() key", key, memberName);
            return key;
        }
    }
}