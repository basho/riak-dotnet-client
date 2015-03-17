// <copyright file="DataTypes.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.DataModeling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;
    using RiakClient.Models.RiakDt;

    /*
     * http://docs.basho.com/riak/latest/dev/using/data-types/
     */
    public sealed class DataTypes : ExampleBase
    {
        [Test]
        public void CountersIdAndFetch()
        {
            id = new RiakObjectId("counters", "counters", "<insert_key_here>");
            Assert.AreEqual("counters", id.BucketType);
            Assert.AreEqual("counters", id.Bucket);
            Assert.AreEqual("<insert_key_here>", id.Key);

            var rslt = client.DtFetchCounter(id);
            CheckResult(rslt.Result);
        }
    }
}
