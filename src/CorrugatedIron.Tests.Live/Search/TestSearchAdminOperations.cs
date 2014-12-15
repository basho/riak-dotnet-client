using System;
using System.Linq;
using System.Threading;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.Search
{
    [TestFixture]
    public class TestSearchAdminOperations : LiveRiakConnectionTestBase
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            //var index = new SearchIndex(Index);
            //Client.PutSearchIndex(index);
            //var props = Client.GetBucketProperties(BucketType, Bucket).Value;
            //props.SetSearchIndex(Index);
            //Client.SetBucketProperties(BucketType, Bucket, props);

        }

        private const string BucketType = "search_type";
        private const string Bucket = "yoko_bucket";
        private const string Index = "yoko_index";
        private const string RiakSearchKey = "a.hacker";
        private const string RiakSearchKey2 = "a.public";
        private readonly Random _random = new Random();


        [Test]
        public void TestFetchAndStoreDefaultSchema()
        {
            // Get Default schema
            var getSchemaResult = Client.GetSearchSchema("_yz_default");
            var defaultSchema = getSchemaResult.Value;

            defaultSchema.Name.ShouldNotBeNull();
            defaultSchema.Content.ShouldNotBeNull();

            // Store as new schema
            var newSchemaName = "test_schema" + _random.Next();
            const string randomComment = "<!-- Random Comment -->";
            var newSchemaContent = defaultSchema.Content + randomComment;
            var newSchema = new SearchSchema(newSchemaName, newSchemaContent);
            
            var putSchemaResult = Client.PutSearchSchema(newSchema);
            putSchemaResult.IsSuccess.ShouldBeTrue(putSchemaResult.ErrorMessage);

            // Fetch new schema and compare
            var getSchemaResult2 = Client.GetSearchSchema(newSchemaName);
            var fetchedNewSchema = getSchemaResult2.Value;

            Assert.AreEqual(newSchemaName, fetchedNewSchema.Name);
            Assert.AreNotEqual(defaultSchema.Content, fetchedNewSchema.Content); // Should differ by the added comment
            Assert.AreEqual(newSchemaContent, fetchedNewSchema.Content);
            Assert.IsTrue(fetchedNewSchema.Content.Contains(randomComment));
        }

        [Test]
        public void TestStoreAndFetchIndex()
        {
            var indexName = "index" + _random.Next();
            var index = new SearchIndex(indexName, RiakConstants.Defaults.YokozunaIndex.IndexName, 2);

            var putIndexResult = Client.PutSearchIndex(index);

            Assert.True(putIndexResult.IsSuccess, "Index Not Created: {0}", putIndexResult.ErrorMessage);
            Func<RiakResult<SearchIndexResult>> func = () => Client.GetSearchIndex(indexName);
            var getIndexResult = func.WaitUntil();

            Assert.True(getIndexResult.IsSuccess, "Index Not Fetched: {0}", getIndexResult.ErrorMessage);
            Assert.AreEqual(1, getIndexResult.Value.Indices.Count);
            var fetchedIndex = getIndexResult.Value.Indices.First();
            Assert.AreEqual(indexName, fetchedIndex.Name);
            Assert.AreEqual(2, fetchedIndex.NVal);
        }


        [Test]
        public void TestDeleteIndex()
        {
            var indexName = "index" + _random.Next();
            var index = new SearchIndex(indexName);
            var putIndexResult = Client.PutSearchIndex(index);

            Assert.True(putIndexResult.IsSuccess, "Index Not Created: {0}", putIndexResult.ErrorMessage);

            Func<RiakResult> deleteIndex = () => Client.DeleteSearchIndex(indexName);
            var deleteIndexResult = deleteIndex.WaitUntil();
            
            Assert.True(deleteIndexResult.IsSuccess, "Index Not Deleted: {0}", deleteIndexResult.ErrorMessage);

        }
    }
}
