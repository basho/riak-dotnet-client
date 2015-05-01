// <copyright file="RiakDtTests.cs" company="Basho Technologies, Inc.">
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

using RiakClient.Tests.Live;
using NUnit.Framework;

namespace RiakClient.Tests.Live
{

    [TestFixture]
    public class RiakDtTests : LiveRiakConnectionTestBase
    {
        private const string Bucket = "riak_dt_bucket";

        /// <summary>
        /// The tearing of the down, it is done here.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
    }
}
