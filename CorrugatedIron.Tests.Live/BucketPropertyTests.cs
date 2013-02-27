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
using CorrugatedIron.Models.Search;
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
        public void SettingExtendedPropertiesToBucketWithSlashesInNameShouldReturnError()
        {
            const string bucketName = "not/valid/here";
            var props = new RiakBucketProperties()
                .SetNVal(4)
                .SetSearch(true)
                .SetWVal("all")
                .SetRVal("quorum");

            var setResult = Client.SetBucketProperties(bucketName, props);
            setResult.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void GettingExtendedPropertiesOnABucketWithoutExtendedPropertiesSetDoesntThrowAnException()
        {
            var bucketName = Guid.NewGuid().ToString();

            var getResult = Client.GetBucketProperties(bucketName, true);
            getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);
        }

        [Test]
        public void GettingPropsOnInvalidBucketStraightAfterSettingDoesntThrowAnException()
        {
            // this bucket name must have ONE slash in it. If it has more or less then
            // errors will come out as expected. If there's one, then Riak thinks we're putting
            // a value in the cluster and so the operation works.
            const string bucketName = "slartibartfast/dentartherdent";
            var props = new RiakBucketProperties()
                .SetNVal(4)
                .SetSearch(true)
                .SetWVal("all")
                .SetRVal("quorum");

            var setResult = Client.SetBucketProperties(bucketName, props);
            setResult.IsSuccess.ShouldBeFalse();

            // this shouldn't throw any exceptions
            var getResult = Client.GetBucketProperties(bucketName, true);
            getResult.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void GettingExtendedPropertiesOnInvalidBucketReturnsError()
        {
            const string bucketName = "this/is/not/a/valid/bucket";

            var getResult = Client.GetBucketProperties(bucketName, true);
            getResult.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void SettingSearchOnRiakBucketMakesBucketSearchable()
        {
            var bucket = Guid.NewGuid().ToString();
            var key = Guid.NewGuid().ToString();
            var props = Client.GetBucketProperties(bucket, true).Value;
            props.SetSearch(true);

            var setResult = Client.SetBucketProperties(bucket, props);
            setResult.IsSuccess.ShouldBeTrue(setResult.ErrorMessage);

            var obj = new RiakObject(bucket, key, new { name = "OJ", age = 34 });
            var putResult = Client.Put(obj);
            putResult.IsSuccess.ShouldBeTrue(putResult.ErrorMessage);

            var q = new RiakFluentSearch(bucket, "name")
                .Search("OJ")
                .And("age", "34")
                .Build();

            var search = new RiakSearchRequest
            {
                Query = q
            };

            var searchResult = Client.Search(search);
            searchResult.IsSuccess.ShouldBeTrue(searchResult.ErrorMessage);
            searchResult.Value.NumFound.ShouldEqual(1u);
            searchResult.Value.Documents[0].Fields.Count.ShouldEqual(3);
            searchResult.Value.Documents[0].Fields.First(x => x.Key == "id").Value.ShouldEqual(key);
        }

        [Test]
        public void SettingPropertiesOnNewBucketWorksCorrectly()
        {
            var bucketName = Guid.NewGuid().ToString();
            var props = new RiakBucketProperties()
                .SetNVal(4)
                .SetSearch(true)
                .SetWVal("all")
                .SetRVal("quorum");

            var setResult = Client.SetBucketProperties(bucketName, props);
            setResult.IsSuccess.ShouldBeTrue(setResult.ErrorMessage);

            var getResult = Client.GetBucketProperties(bucketName, true);
            getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);

            props = getResult.Value;
            props.NVal.HasValue.ShouldBeTrue();
            props.NVal.Value.ShouldEqual(4U);
            props.SearchEnabled.ShouldBeTrue();
            props.WVal.Right.ShouldEqual("all");
            props.RVal.Right.ShouldEqual("quorum");
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
            var task = Client.Async.Batch(batch =>
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
                .SetDwVal(10)
                .SetWVal(5)
                .SetLastWriteWins(true);

            var setPropsResult = Client.SetBucketProperties(bucket, props);
            setPropsResult.IsSuccess.ShouldBeTrue(setPropsResult.ErrorMessage);

            var resetResult = Client.ResetBucketProperties(bucket);
            resetResult.IsSuccess.ShouldBeTrue(resetResult.ErrorMessage);

            var getPropsResult = Client.GetBucketProperties(bucket, true);
            getPropsResult.IsSuccess.ShouldBeTrue(getPropsResult.ErrorMessage);

            var resetProps = getPropsResult.Value;

            resetProps.AllowMultiple.ShouldNotEqual(props.AllowMultiple);
            resetProps.DwVal.ShouldNotEqual(props.DwVal);
            resetProps.WVal.ShouldNotEqual(props.WVal);
            resetProps.LastWriteWins.ShouldNotEqual(props.LastWriteWins);
        }
    }

}
