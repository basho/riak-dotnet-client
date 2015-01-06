using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.Extensions;
using CorrugatedIron.Util;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.MapReduce
{
    [TestFixture]
    public class WhenUsingFluentKeyFilters : RiakMapReduceTestBase
    {

        [SetUp]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            Client = Cluster.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }

        [Test]
        public void EqualsFindsOneKey()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery {ContentType = MrContentType};

            mr.Inputs(Bucket)
                .Filter(f => f.Equal("time_8"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Values.Count().ShouldEqual(0);
            mrResult.PhaseResults.ElementAt(1).Values.Count().ShouldNotEqual(0);
            
            
            var values = JsonConvert.DeserializeObject<int[]>(mrResult.PhaseResults.ElementAt(1).Values.First().FromRiakString());
            values[0].ShouldEqual(1);
        }

        [Test]
        public void StartsWithFindsAllKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery { ContentType = MrContentType };

            mr.Inputs(Bucket)
                .Filter(f => f.StartsWith("time"))
                .MapJs(m => m.Source("function (o) { return [1]; }"))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            mrResult.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrResult.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            mrResult.PhaseResults.ElementAt(0).Values.Count().ShouldEqual(0);
            mrResult.PhaseResults.ElementAt(1).Values.Count().ShouldNotEqual(0);
   
            
            var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            values[0].ShouldEqual(10);
        }

        [Test]
        public void StartsWithAndBetweenReturnASubsetOfAllKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                Client.Put(new RiakObject(Bucket, string.Format("time_{0}", i), EmptyBody, RiakConstants.ContentTypes.ApplicationJson));
            }

            var mr = new RiakMapReduceQuery { ContentType = MrContentType };

            mr.Inputs(Bucket)
                .Filter(f => f.And(l => l.StartsWith("time"),
                    r => r.Tokenize("_", 2)
                        .StringToInt()
                        .Between(3, 7, true)))
                .MapJs(m => m.Source("function (o) { return [1]; }").Keep(false))
                .ReduceJs(r => r.Name("Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();

            var mrResult = result.Value;
            mrResult.PhaseResults.ShouldNotBeNull();
            mrResult.PhaseResults.Count().ShouldEqual(2);

            var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            values[0].ShouldEqual(5);
        }
    }
}