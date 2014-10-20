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

using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.CommitHook;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;
using System;
using System.Linq;

namespace CorrugatedIron.Tests.Live.BucketPropertyTests
{
    [TestFixture]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        protected string Bucket = "test_bucket_bucket_properties";

        // use the one node configuration here because we might run the risk
        // of hitting different nodes in the configuration before the props
        // are replicated to other nodes.
        public WhenDealingWithBucketProperties()
            : base("riak1NodeConfiguration")
        {
        }

        [Test]
        public void ListKeysReturnsAllkeys()
        {
            Func<string> generator = () => Guid.NewGuid().ToString();
            var bucket = string.Format("{0}_{1}", Bucket, generator());
            var pairs = generator
                .Replicate(10)
                .Select(f => new RiakObject(bucket, f(), "foo", RiakConstants.ContentTypes.TextPlain))
                .ToList();

            var postResults = Client.Put(pairs);

            var results = Client.ListKeys(bucket).ToList();



            results.Count().ShouldEqual(10);
        }

        [Test]
        public void SettingExtendedPropertiesToBucketWithSlashesInNameShouldReturnError()
        {
            var bucketName = string.Format("{0}_{1}", Bucket, "not/valid/here");
            var props = new RiakBucketProperties()
                .SetNVal(4)
                .SetSearch(true)
                .SetWVal("all")
                .SetRVal("quorum");

            RiakException exception = null;
            try
            {
                var getResult = Client.SetBucketProperties(bucketName, props);
            }
            catch (RiakException riakException)
            {
                exception = riakException;
            }

            exception.ShouldNotBeNull();
            exception.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GettingExtendedPropertiesOnABucketWithoutExtendedPropertiesSetDoesntThrowAnException()
        {
            var bucketName = string.Format("{0}_{1}", Bucket, Guid.NewGuid());
            var getResult = Client.GetBucketProperties(bucketName);

            getResult.ShouldNotBeNull();
        }



        [Test]
        public void GettingExtendedPropertiesOnInvalidBucketReturnsError()
        {
            var bucketName = string.Format("{0}_{1}", Bucket, "this/is/not/a/valid/bucket");
            RiakException exception = null;
            try
            {
                var getResult = Client.GetBucketProperties(bucketName);
            }
            catch (RiakException riakException)
            {
                exception = riakException;
            }

            exception.ShouldNotBeNull();
            exception.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void SettingSearchOnRiakBucketMakesBucketSearchable()
        {
            var bucket = string.Format("{0}_{1}", Bucket, Guid.NewGuid());
            var key = Guid.NewGuid().ToString();
            var props = Client.GetBucketProperties(bucket);
            props.SetSearch(true);

            var setResult = Client.SetBucketProperties(bucket, props);
            setResult.ShouldBeTrue();

            var obj = new RiakObject(bucket, key, new { name = "OJ", age = 34 });
            var putResult = Client.Put(obj);
            putResult.ShouldNotBeNull();

            var q = new RiakFluentSearch(bucket, "name")
                .Search("OJ")
                .And("age", "34")
                .Build();

            var search = new RiakSearchRequest
            {
                Query = q
            };

            var searchResult = Client.Search(search);
            searchResult.ShouldNotBeNull();
            searchResult.NumFound.ShouldEqual(1u);
            searchResult.Documents[0].Fields.Count.ShouldEqual(3);
            searchResult.Documents[0].Fields.First(x => x.Key == "id").Value.ShouldEqual(key);
        }

        [Test]
        public void SettingPropertiesOnNewBucketWorksCorrectly()
        {
            var bucketName = string.Format("{0}_{1}", Bucket, Guid.NewGuid());

            var props = new RiakBucketProperties()
                .SetNVal(4)
                .SetSearch(true)
                .SetWVal("all")
                .SetRVal("quorum");

            var setResult = Client.SetBucketProperties(bucketName, props);
            setResult.ShouldBeTrue();

            var getResult = Client.GetBucketProperties(bucketName);
            getResult.ShouldNotBeNull();

            props = getResult;
            props.NVal.HasValue.ShouldBeTrue();
            props.NVal.Value.ShouldEqual(4U);
            props.Search.ShouldNotEqual(null);
            props.Search.Value.ShouldBeTrue();
            props.WVal.ShouldEqual(RiakConstants.QuorumOptionsLookup["all"]);
            props.RVal.ShouldEqual(RiakConstants.QuorumOptionsLookup["quorum"]);
        }

        [Test]
        public void GettingWithExtendedFlagReturnsExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket);
            result.AllowMultiple.HasValue.ShouldBeTrue();
            result.NVal.HasValue.ShouldBeTrue();
            result.LastWriteWins.HasValue.ShouldBeTrue();
            result.RVal.ShouldNotEqual(null);
            result.RwVal.ShouldNotEqual(null);
            result.DwVal.ShouldNotEqual(null);
            result.WVal.ShouldNotEqual(null);
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperly()
        {
            // make sure we're all clear first
            var result = Client.GetBucketProperties(PropertiesTestBucket);

            var props = result;
            props.ClearPostCommitHooks().ClearPreCommitHooks();
            Client.SetBucketProperties(PropertiesTestBucket, props).ShouldBeTrue();

            // when we load, the commit hook lists should be null
            result = Client.GetBucketProperties(PropertiesTestBucket);
            props = result;
            props.PreCommitHooks.ShouldBeNull();
            props.PostCommitHooks.ShouldBeNull();

            // we then store something in each
            props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));

            Client.SetBucketProperties(PropertiesTestBucket, props).ShouldBeTrue();

            // load them out again and make sure they got loaded up
            result = Client.GetBucketProperties(PropertiesTestBucket);
            props = result;

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
                result.ShouldNotBeNull();
                var props = result;
                props.ClearPostCommitHooks().ClearPreCommitHooks();
                var propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                propResult.ShouldBeTrue();

                // when we load, the commit hook lists should be null
                result = batch.GetBucketProperties(PropertiesTestBucket);
                result.ShouldNotBeNull();
                props = result;
                props.PreCommitHooks.ShouldBeNull();
                props.PostCommitHooks.ShouldBeNull();

                // we then store something in each
                props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                    .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                    .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                propResult = batch.SetBucketProperties(PropertiesTestBucket, props);
                propResult.ShouldBeTrue();

                // load them out again and make sure they got loaded up
                result = batch.GetBucketProperties(PropertiesTestBucket);
                result.ShouldNotBeNull();
                props = result;

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
                var result = batch.GetBucketProperties(PropertiesTestBucket).ConfigureAwait(false).GetAwaiter().GetResult();
                result.ShouldNotBeNull();
                var props = result;
                props.ClearPostCommitHooks().ClearPreCommitHooks();
                var propResult = batch.SetBucketProperties(PropertiesTestBucket, props).ConfigureAwait(false).GetAwaiter().GetResult();
                propResult.ShouldBeTrue();

                // when we load, the commit hook lists should be null
                result = batch.GetBucketProperties(PropertiesTestBucket).ConfigureAwait(false).GetAwaiter().GetResult();
                result.ShouldNotBeNull();
                props = result;
                props.PreCommitHooks.ShouldBeNull();
                props.PostCommitHooks.ShouldBeNull();

                // we then store something in each
                props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                    .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                    .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                propResult = batch.SetBucketProperties(PropertiesTestBucket, props).ConfigureAwait(false).GetAwaiter().GetResult();
                propResult.ShouldBeTrue();

                // load them out again and make sure they got loaded up
                result = batch.GetBucketProperties(PropertiesTestBucket).ConfigureAwait(false).GetAwaiter().GetResult();
                result.ShouldNotBeNull();
                props = result;

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
            var bucket = string.Format("{0}_{1}", Bucket, "Schmoopy");

            var props = new RiakBucketProperties()
                .SetAllowMultiple(true)
                .SetDwVal(10)
                .SetWVal(5)
                .SetLastWriteWins(true);

            Client.SetBucketProperties(bucket, props).ShouldBeTrue();

            Client.ResetBucketProperties(bucket).ShouldBeTrue();

            var getPropsResult = Client.GetBucketProperties(bucket);
            getPropsResult.ShouldNotBeNull();

            var resetProps = getPropsResult;

            resetProps.AllowMultiple.ShouldNotEqual(props.AllowMultiple);
            resetProps.DwVal.ShouldNotEqual(props.DwVal);
            resetProps.WVal.ShouldNotEqual(props.WVal);
            resetProps.LastWriteWins.ShouldNotEqual(props.LastWriteWins);
        }
    }

}
