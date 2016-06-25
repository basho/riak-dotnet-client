namespace RiakClientTests.Live.BucketPropertyTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.CommitHook;
    using RiakClient.Util;

    [TestFixture, IntegrationTest, SkipMono]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        private readonly Random _random = new Random();

        private string bucket;

        [SetUp]
        public void TestSetUp()
        {
            bucket = Guid.NewGuid().ToString();
        }

        [Test]
        public void GettingExtendedPropertiesOnABucketWithoutExtendedPropertiesSetDoesntThrowAnException()
        {
            var getResult = Client.GetBucketProperties(bucket);
            getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);
        }

        [Test]
        public void SettingPropertiesOnNewBucketWorksCorrectly()
        {
            RiakBucketProperties props = new RiakBucketProperties()
                .SetNVal((NVal)4)
                .SetLegacySearch(true)
                .SetW((Quorum)"all")
                .SetR((Quorum)"quorum");

            var setResult = Client.SetBucketProperties(bucket, props);
            setResult.IsSuccess.ShouldBeTrue(setResult.ErrorMessage);

            Func<RiakResult<RiakBucketProperties>> getFunc = () =>
                {
                    var getResult = Client.GetBucketProperties(bucket);
                    getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);
                    return getResult;
                };

            Func<RiakResult<RiakBucketProperties>, bool> successFunc = (r) =>
                {
                    bool rv = false;

                    RiakBucketProperties p = r.Value;
                    if (p.NVal == 4)
                    {
                        rv = true;
                        props = p;
                    }

                    return rv;
                };

            getFunc.WaitUntil(successFunc);

            props.NVal.ShouldNotBeNull();
            ((int)props.NVal).ShouldEqual(4);
            props.LegacySearch.ShouldNotEqual(null);
            props.LegacySearch.Value.ShouldBeTrue();
            props.W.ShouldEqual(Quorum.WellKnown.All);
            props.R.ShouldEqual(Quorum.WellKnown.Quorum);
        }

        [Test]
        public void GettingWithExtendedFlagReturnsExtraProperties()
        {
            var result = Client.GetBucketProperties(bucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.ShouldNotBeNull();
            result.Value.LastWriteWins.HasValue.ShouldBeTrue();
            result.Value.R.ShouldNotEqual(null);
            result.Value.Rw.ShouldNotEqual(null);
            result.Value.Dw.ShouldNotEqual(null);
            result.Value.W.ShouldNotEqual(null);
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperly()
        {
            // when we load, the commit hook lists should be null
            RiakResult<RiakBucketProperties> result = Client.GetBucketProperties(bucket);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            RiakBucketProperties props = result.Value;
            props.PreCommitHooks.ShouldBeNull();
            props.PostCommitHooks.ShouldBeNull();

            // we then store something in each
            props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));

            var propResult = Client.SetBucketProperties(bucket, props);
            propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

            // load them out again and make sure they got loaded up
            Func<RiakResult<RiakBucketProperties>> getFunc = () =>
                {
                    result = Client.GetBucketProperties(bucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    return result;
                };

            Func<RiakResult<RiakBucketProperties>, bool> successFunc = (r) =>
                {
                    bool rv = false;

                    RiakBucketProperties p = r.Value;
                    if (EnumerableUtil.NotNullOrEmpty(p.PreCommitHooks) &&
                        EnumerableUtil.NotNullOrEmpty(p.PostCommitHooks))
                    {
                        rv = true;
                        props = p;
                    }

                    return rv;
                };

            getFunc.WaitUntil(successFunc);

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
                    var result = batch.GetBucketProperties(bucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    var props = result.Value;
                    props.ClearPostCommitHooks().ClearPreCommitHooks();
                    var propResult = batch.SetBucketProperties(bucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // when we load, the commit hook lists should be null
                    result = batch.GetBucketProperties(bucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;
                    props.PreCommitHooks.ShouldBeNull();
                    props.PostCommitHooks.ShouldBeNull();

                    // we then store something in each
                    props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                        .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                        .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                    propResult = batch.SetBucketProperties(bucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // load them out again and make sure they got loaded up
                    result = batch.GetBucketProperties(bucket);
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
                    var result = batch.GetBucketProperties(bucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    var props = result.Value;
                    props.ClearPostCommitHooks().ClearPreCommitHooks();
                    var propResult = batch.SetBucketProperties(bucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // when we load, the commit hook lists should be null
                    result = batch.GetBucketProperties(bucket);
                    result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                    props = result.Value;
                    props.PreCommitHooks.ShouldBeNull();
                    props.PostCommitHooks.ShouldBeNull();

                    // we then store something in each
                    props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                        .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                        .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
                    propResult = batch.SetBucketProperties(bucket, props);
                    propResult.IsSuccess.ShouldBeTrue(propResult.ErrorMessage);

                    // load them out again and make sure they got loaded up
                    result = batch.GetBucketProperties(bucket);
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
            var props = new RiakBucketProperties()
                .SetAllowMultiple(true)
                .SetDw(10)
                .SetW(5)
                .SetLastWriteWins(true);

            var setPropsResult = Client.SetBucketProperties(bucket, props);
            setPropsResult.IsSuccess.ShouldBeTrue(setPropsResult.ErrorMessage);

            Func<RiakResult<RiakBucketProperties>> getFunc = () =>
                {
                    var getResult = Client.GetBucketProperties(bucket);
                    getResult.IsSuccess.ShouldBeTrue(getResult.ErrorMessage);
                    return getResult;
                };

            Func<RiakResult<RiakBucketProperties>, bool> successFunc = (r) =>
                {
                    bool rv = false;

                    if (r.Value.AllowMultiple == props.AllowMultiple &&
                        r.Value.LastWriteWins == props.LastWriteWins)
                    {
                        rv = true;
                    }

                    return rv;
                };

            getFunc.WaitUntil(successFunc);

            var resetResult = Client.ResetBucketProperties(bucket);
            resetResult.IsSuccess.ShouldBeTrue(resetResult.ErrorMessage);

            RiakBucketProperties resetProps = null;
            successFunc = (r) =>
                {
                    bool rv = false;

                    if (r.Value.AllowMultiple != props.AllowMultiple &&
                        r.Value.LastWriteWins != props.LastWriteWins)
                    {
                        rv = true;
                        resetProps = r.Value;
                    }

                    return rv;
                };

            getFunc.WaitUntil(successFunc);

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

            new List<RiakConstants.RiakEnterprise.ReplicationMode> {
                RiakConstants.RiakEnterprise.ReplicationMode.True,
                RiakConstants.RiakEnterprise.ReplicationMode.False
            }.ShouldContain(getInitialPropsResponse.Value.ReplicationMode);

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
        public void TestConsistentPropertyOnSCBucketType()
        {
            var regProps = Client.GetBucketProperties("consistent", "bucket");
            regProps.IsSuccess.ShouldBeTrue();
            regProps.Value.Consistent.Value.ShouldBeTrue();
        }
    }
}
