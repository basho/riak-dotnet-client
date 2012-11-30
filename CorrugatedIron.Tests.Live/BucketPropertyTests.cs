// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.CommitHook;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace CorrugatedIron.Tests.Live.BucketPropertyTests
{
    [TestFixture]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        // use the one node configuration here because we might run the risk
        // of hitting different nodes in the configuration before the props
        // are replicated to other nodes.
        public WhenDealingWithBucketProperties()
            :base("riak1NodeConfiguration")
        {
        }

        [Test]
        public void ListKeysReturnsAllkeys()
        {
            Func<string> generator = () => Guid.NewGuid().ToString();
            var bucket = generator();
            var pairs = generator.Replicate(10).Select(f => new RiakObject(bucket, f(), "foo", RiakConstants.ContentTypes.TextPlain)).ToList();
            Client.Put(pairs);

            var results = Client.ListKeys(bucket);
            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.Count().ShouldEqual(10);
        }

        [Test]
        public void GettingWithoutExtendedFlagDoesNotReturnExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.HasValue.ShouldBeTrue();
            result.Value.LastWriteWins.HasValue.ShouldBeFalse();
            result.Value.RVal.ShouldBeNull();
            result.Value.RwVal.ShouldBeNull();
            result.Value.DwVal.ShouldBeNull();
            result.Value.WVal.ShouldBeNull();
        }

        [Test]
        public void GettingWithExtendedFlagReturnsExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.HasValue.ShouldBeTrue();
            result.Value.LastWriteWins.HasValue.ShouldBeTrue();
            result.Value.RVal.ShouldNotBeNull();
            result.Value.RwVal.ShouldNotBeNull();
            result.Value.DwVal.ShouldNotBeNull();
            result.Value.WVal.ShouldNotBeNull();
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperly()
        {
            // make sure we're all clear first
            var result = Client.GetBucketProperties(PropertiesTestBucket, true);
             result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            var props = result.Value;
            props.ClearPostCommitHooks().ClearPreCommitHooks();
            Client.SetBucketProperties(PropertiesTestBucket, props).IsSuccess.ShouldBeTrue();

            // when we load, the commit hook lists should be null
            result = Client.GetBucketProperties(PropertiesTestBucket, true);
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
            result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            props = result.Value;

            props.PreCommitHooks.ShouldNotBeNull();
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.ShouldNotBeNull();
            props.PostCommitHooks.Count.ShouldEqual(1);
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperlyInBatch()
        {
            Client.Batch(batch =>
                {
                    // make sure we're all clear first
                    var result = batch.GetBucketProperties(PropertiesTestBucket, true);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    var props = result.Value;
                    props.ClearPostCommitHooks().ClearPreCommitHooks();
                    var propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // when we load, the commit hook lists should be null
                    result = batch.GetBucketProperties(PropertiesTestBucket, true);
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
                    result = batch.GetBucketProperties(PropertiesTestBucket, true);
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
            var wait = new ManualResetEvent(false);
            Client.Async.Batch(batch =>
                {
                    try
                    {
                        // make sure we're all clear first
                        var result = batch.GetBucketProperties(PropertiesTestBucket, true);
                        result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                        var props = result.Value;
                        props.ClearPostCommitHooks().ClearPreCommitHooks();
                        var propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                        propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                        // when we load, the commit hook lists should be null
                        result = batch.GetBucketProperties(PropertiesTestBucket, true);
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
                        result = batch.GetBucketProperties(PropertiesTestBucket, true);
                        result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                        props = result.Value;

                        props.PreCommitHooks.ShouldNotBeNull();
                        props.PreCommitHooks.Count.ShouldEqual(2);
                        props.PostCommitHooks.ShouldNotBeNull();
                        props.PostCommitHooks.Count.ShouldEqual(1);

                        completed = true;
                    }
                    finally
                    {
                        wait.Set();
                    }
                });

            wait.WaitOne();

            completed.ShouldBeTrue();
        }
    }

}
