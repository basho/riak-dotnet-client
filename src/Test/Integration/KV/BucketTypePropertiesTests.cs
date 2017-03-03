// <copyright file="BucketTypePropertiesTests.cs" company="Basho Technologies, Inc.">
// Copyright 2017 - Basho Technologies, Inc.
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

namespace Test.Integration.KV
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.KV;
    using RiakClient.Models;

    public class BucketTypePropertiesTests : TestBase
    {
        protected override RiakString BucketType
        {
            get { return new RiakString("plain"); }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            RiakMinVersion(2, 0, 0);
        }

        [Test]
        public void Should_Not_Store_For_Default_BucketType()
        {
            RiakBucketProperties props = new RiakBucketProperties().SetHllPrecision(16);
            Assert.Throws<ArgumentNullException>(() => new StoreBucketTypeProperties(RiakConstants.DefaultBucketType, props));
        }

        [Test]
        public void Can_Fetch_BucketType_Properties()
        {
            var fetch = new FetchBucketTypeProperties(BucketType);

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            
            FetchBucketTypePropertiesResponse response = fetch.Response;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.NotFound);
            Assert.IsNotNull(response.Value);

            RiakBucketProperties properties = response.Value;
            Assert.AreEqual(3, (int)properties.NVal);
        }

        [Test]
        public void Can_Store_And_Fetch_BucketType_Properties()
        {
            {
                var nval4Props = new RiakBucketProperties().SetNVal(new NVal(4));
                var storeNval4Props = new StoreBucketTypeProperties(BucketType, nval4Props);

                var storeNval4Result = client.Execute(storeNval4Props);
                Assert.IsTrue(storeNval4Result.IsSuccess, storeNval4Result.ErrorMessage);
                var storeBucketTypePropertiesResponse = storeNval4Props.Response;
                Assert.IsFalse(storeBucketTypePropertiesResponse.NotFound);
            }

            {
                var fetch = new FetchBucketTypeProperties(BucketType);
                var fetchResult = client.Execute(fetch);
                var fetchResponse = fetch.Response;
                Assert.IsTrue(fetchResult.IsSuccess, fetchResult.ErrorMessage);
                Assert.AreEqual(4, (int)fetchResponse.Value.NVal);
            }

            {
                var nval3Props = new RiakBucketProperties().SetNVal(new NVal(3));
                var storeNval3Props = new StoreBucketTypeProperties(BucketType, nval3Props);
                var storeNval3Result = client.Execute(storeNval3Props);
                Assert.IsTrue(storeNval3Result.IsSuccess, storeNval3Result.ErrorMessage);
            }
        }
    }
}
