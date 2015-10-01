namespace RiakClient.Tests.Live
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient.Comms;
    using RiakClient.Models.RiakSearch;
    using RiakClient.Models.RiakSearch.Solr;
    using RiakClient.Util;
    using RiakClient.Models;
    using RiakClient.Models.MapReduce;
    using RiakClient.Models.MapReduce.Inputs;
    using RiakClient.Tests.Extensions;

    [TestFixture]
    public class SolrTests : RiakMapReduceTests
    {
        // N.B. You need to install the search hooks on the riak_search_bucket first via `bin/search-cmd install riak_search_bucket`
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private const string RiakSearchDoc = "{\"name\":\"Alyssa P. Hacker\", \"bio\":\"I'm an engineer, making awesome things.\", \"favorites\":{\"book\":\"The Moon is a Harsh Mistress\",\"album\":\"Magical Mystery Tour\", }}";
        private const string RiakSearchDoc2 = "{\"name\":\"Alan Q. Public\", \"bio\":\"I'm an exciting mathematician\", \"favorites\":{\"book\":\"Prelude to Mathematics\",\"album\":\"The Fame Monster\"}}";

        public SolrTests ()
        {
            Bucket = "riak_search_bucket";
        }

        [SetUp]
        public void SetUp() 
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            Client = Cluster.CreateClient();
            
            var props = Client.GetBucketProperties(Bucket, true).Value;
            props.SetSearch(true);
            Client.SetBucketProperties(Bucket, props);
        }
        
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
        
        [Test]
        public void SearchingByNameReturnsTheObjectId()
        {
            Client.Put(new RiakObject(Bucket, RiakSearchKey, RiakSearchDoc, RiakConstants.ContentTypes.ApplicationJson));
            Client.Put(new RiakObject(Bucket, RiakSearchKey2, RiakSearchDoc2, RiakConstants.ContentTypes.ApplicationJson));
            
            var mr = new RiakMapReduceQuery();

            var token = new RiakSearchPhraseToken("Al*");
            var solr = new SolrQuery { Fieldname = "name", QueryPart = token };

            var modFunArg = new RiakModuleFunctionArgInput
                                {
                                    Module = "riak_search",
                                    Function = "mapred_search",
                                    Arg = new[] {Bucket, solr.ToString()}
                                };
                                
            mr.Inputs(modFunArg)
                .MapJs(m => m.Source(@"function(value, keydata, arg) { return [value]; }").Keep(true))
                .ReduceJs(r => r.Source(@"function(values, arg) { return values; }").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            
            var mrResult = result.Value;
            mrResult.PhaseResults.Count().ShouldEqual(2);
            
            mrResult.PhaseResults.ElementAt(0).Values.ShouldNotBeNull();
            mrResult.PhaseResults.ElementAt(1).Values.ShouldNotBeNull();
            // TODO Add data introspection to test - need to verify the results, after all.
        }
    }
}

