using System;
using CorrugatedIron.Models.Search;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.Search
{
    [TestFixture]
    public class TestYzAdminOperations : LiveRiakConnectionTestBase
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
    }
}
