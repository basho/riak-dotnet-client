// <copyright file="GitHub_277.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace Test.Integration.Issues
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    public class GitHub_277 : TestBase
    {
        protected override RiakString BucketType
        {
            get { return new RiakString("write_once"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("github_277"); }
        }

        [Test]
        public void Putting_A_Value_To_A_Write_Once_Bucket_Works()
        {
            string key = Guid.NewGuid().ToString();
            string value = "test value";

            var r = client.GetBucketProperties(BucketType, Bucket);

            // TODO FUTURE - someday Riak will return useful error codes
            if (!r.IsSuccess && r.ErrorMessage.ToLowerInvariant().Contains("no bucket-type named"))
            {
                Assert.Pass("write_once bucket type not available, skipping");
            }
            else
            {
                var id = new RiakObjectId(BucketType, Bucket, key);
                var obj = new RiakObject(id, value, RiakConstants.ContentTypes.TextPlain);

                RiakResult<RiakObject> rslt = client.Put(obj);
                Assert.True(rslt.IsSuccess, rslt.ErrorMessage);
            }
        }
    }
}
