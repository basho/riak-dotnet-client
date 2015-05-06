// <copyright file="IntegrationTestExtensionsTest.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live
{
    using System;
    using Extensions;
    using NUnit.Framework;
    using RiakClient;

    [TestFixture]
    public class IntegrationTestExtensionsTest
    {
        [Ignore("Run this to test the WaitUntil test helper's output")]
        [Test]
        public void ThisTestShouldFail()
        {
            Func<RiakResult> alwaysFail = () => RiakResult.FromError(ResultCode.InvalidRequest, "Nope.", true);
            Func<RiakResult> alwaysThrow = () => { throw new ApplicationException("Whoopsie"); };
            var failResult = alwaysFail.WaitUntil(2);
            alwaysThrow.WaitUntil(2);
            failResult.IsSuccess.ShouldBeFalse();
        }
    }
}

