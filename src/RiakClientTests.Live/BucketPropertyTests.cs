// <copyright file="BucketPropertyTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live.BucketPropertyTests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;
    using RiakClient.Models.CommitHook;
    using RiakClient.Util;

    [TestFixture]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        private readonly Random _random = new Random();

        [Test]
        public void GettingExtendedPropertiesOnABucketWithoutExtendedPropertiesSetDoesntThrowAnException()
        {
            var bucketName = Guid.NewGuid().ToString();

            var getResult = Client.GetBucketProperties(bucketName);
            getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);
        }

        [Test]
        public void SettingPropertiesOnNewBucketWorksCorrectly()
        {
            var bucketName = Guid.NewGuid().ToString();
            RiakBucketProperties props = new RiakBucketProperties()
                .SetNVal(4)
                .SetLegacySearch(true)
                .SetW((Quorum)"all")
                .SetR((Quorum)"quorum");

            var setResult = Client.SetBucketProperties(bucketName, props);
            setResult.IsSuccess.ShouldBeTrue(setResult.ErrorMessage);

            var getResult = Client.GetBucketProperties(bucketName);
            getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);

            props = getResult.Value;
            props.NVal.HasValue.ShouldBeTrue();
            props.NVal.Value.ShouldEqual(4U);
            props.LegacySearch.ShouldNotEqual(null);
            props.LegacySearch.Value.ShouldBeTrue();
            props.W.ShouldEqual(Quorum.WellKnown.All);
            props.R.ShouldEqual(Quorum.WellKnown.Quorum);
        }

        [Test]
        public void GettingWithExtendedFlagReturnsExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.HasValue.ShouldBeTrue();
            result.Value.LastWriteWins.HasValue.ShouldBeTrue();
            result.Value.R.ShouldNotEqual(null);
            result.Value.Rw.ShouldNotEqual(null);
            result.Value.Dw.ShouldNotEqual(null);
            result.Value.W.ShouldNotEqual(null);
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperly()
        {
            // make sure we're all clear first
            var result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var props = result.Value;
            props.ClearPostCommitHooks().ClearPreCommitHooks();
            Client.SetBucketProperties(PropertiesTestBucket, props).IsSuccess.ShouldBeTrue();

            // when we load, the commit hook lists should be null
            result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            props = result.Value;
            props.PreCommitHooks.ShouldBeNull();
            props.PostCommitHooks.ShouldBeNull();

            // we then store something in each
            props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));

            var propResult = Client.SetBucketProperties(PropertiesTestBucket, props);
            propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

            // load them out again and make sure they got loaded up
            result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            props = result.Value;

            props.PreCommitHooks.ShouldNotBeNull();
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.ShouldNotBeNull();
            props.PostCommitHooks.Count.ShouldEqual(1);

            Client.DeleteBucket(PropertiesTestBucket);
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperlyInBatch()
        {
            Client.Batch(batch =>
                {
                    // make sure we're all clear first
                    var result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    var props = result.Value;
                    props.ClearPostCommitHooks().ClearPreCommitHooks();
                    var propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // when we load, the commit hook lists should be null
                    result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;
                    props.PreCommitHooks.ShouldBeNull();
                    props.PostCommitHooks.ShouldBeNull();

                    // we then store something in each
                    props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                        .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                        .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                    propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // load them out again and make sure they got loaded up
                    result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;

                    props.PreCommitHooks.ShouldNotBeNull();
                    props.PreCommitHooks.Count.ShouldEqual(2);
                    props.PostCommitHooks.ShouldNotBeNull();
                    props.PostCommitHooks.Count.ShouldEqual(1);
                });
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperlyInAsyncBatch()
        {
            var completed = false;
            var task = Client.Async.Batch(batch =>
                {
                    // make sure we're all clear first
                    var result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    var props = result.Value;
                    props.ClearPostCommitHooks().ClearPreCommitHooks();
                    var propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // when we load, the commit hook lists should be null
                    result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;
                    props.PreCommitHooks.ShouldBeNull();
                    props.PostCommitHooks.ShouldBeNull();

                    // we then store something in each
                    props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                        .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                        .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                    propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // load them out again and make sure they got loaded up
                    result = batch.GetBucketProperties(PropertiesTestBucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;

                    props.PreCommitHooks.ShouldNotBeNull();
                    props.PreCommitHooks.Count.ShouldEqual(2);
                    props.PostCommitHooks.ShouldNotBeNull();
                    props.PostCommitHooks.Count.ShouldEqual(1);

                    completed = true;
                });

            task.Wait();

            completed.ShouldBeTrue();
        }

        [Test]
        public void ResettingBucketPropertiesWorksCorrectly()
        {
            const string bucket = "Schmoopy";

            var props = new RiakBucketProperties()
                .SetAllowMultiple(true)
                .SetDw(10)
                .SetW(5)
                .SetLastWriteWins(true);

            var setPropsResult = Client.SetBucketProperties(bucket, props);
            setPropsResult.IsSuccess.ShouldBeTrue(setPropsResult.ErrorMessage);

            var resetResult = Client.ResetBucketProperties(bucket);
            resetResult.IsSuccess.ShouldBeTrue(resetResult.ErrorMessage);

            var getPropsResult = Client.GetBucketProperties(bucket);
            getPropsResult.IsSuccess.ShouldBeTrue(getPropsResult.ErrorMessage);

            var resetProps = getPropsResult.Value;

            resetProps.AllowMultiple.ShouldNotEqual(props.AllowMultiple);
            resetProps.Dw.ShouldNotEqual(props.Dw);
            resetProps.W.ShouldNotEqual(props.W);
            resetProps.LastWriteWins.ShouldNotEqual(props.LastWriteWins);
        }

        [Test]
        public void TestNewBucketGivesReplFlagBack()
        {
            var bucket = "replicants" + _random.Next();
            var getInitialPropsResponse = Client.GetBucketProperties(bucket);

            if (Client.GetServerStatus().Value.Contains("riak_repl_version"))
            {
                getInitialPropsResponse.Value.ReplicationMode.ShouldEqual(RiakConstants.RiakEnterprise.ReplicationMode.True);
            }
            else
            {
                getInitialPropsResponse.Value.ReplicationMode.ShouldEqual(RiakConstants.RiakEnterprise.ReplicationMode.False);
            }
        }

        [Test]
        public void TestBucketTypesPropertyWorks()
        {
            var setsBucketTypeBucketPropsResult = Client.GetBucketProperties(BucketTypeNames.Sets, "Schmoopy");
            setsBucketTypeBucketPropsResult.Value.DataType.ShouldEqual("set");

            var plainBucketTypeBucketPropsResult = Client.GetBucketProperties("plain", "Schmoopy");
            plainBucketTypeBucketPropsResult.Value.DataType.ShouldBeNull();
        }

        [Test]
        public void TestConsistentPropertyOnRegularBucketType()
        {
            var regProps = Client.GetBucketProperties("plain", "bucket");
            regProps.IsSuccess.ShouldBeTrue();
            regProps.Value.Consistent.Value.ShouldBeFalse();
        }

        [Test]
        public void TestConsistentPropertyIsNullOnNewProps()
        {
            var props = new RiakBucketProperties();
            props.Consistent.HasValue.ShouldBeFalse();
        }

        [Test]
        [Ignore("Test will only pass if you have a bucket type named \"consistent\" with SC enabled on it.")]
        public void TestConsistentPropertyOnSCBucketType()
        {
            var regProps = Client.GetBucketProperties("consistent", "bucket");
            regProps.IsSuccess.ShouldBeTrue();
            regProps.Value.Consistent.Value.ShouldBeTrue();
        }
    }
}
