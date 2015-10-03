namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using NUnit.Framework;
    using RiakClient.Models.Search;

    /*
     * http://docs.basho.com/riak/latest/dev/search/document-store/
     */
    public sealed class DocumentStore : ExampleBase
    {
        const string blogPostSchemaName = "blog_post_schema";
        const string blogPostSchemaFileName = "blog_post_schema.xml";
        private static readonly Uri blogPostSchema =
            new Uri("https://github.com/basho/basho_docs/raw/master/source/data/blog_post_schema.xml");

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            if (!File.Exists(blogPostSchemaFileName))
            {
                Console.WriteLine("Writing {0} in {1}", blogPostSchemaFileName, Environment.CurrentDirectory);
                var req = WebRequest.Create(blogPostSchema);
                var rsp = req.GetResponse();
                var stream = rsp.GetResponseStream();
                string line = string.Empty;
                using (var writer = new StreamWriter(blogPostSchemaFileName))
                {
                    using (var rdr = new StreamReader(stream))
                    {
                        while ((line = rdr.ReadLine()) != null)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
            }

            base.CreateClient();

            var getSchemaResult = client.GetSearchSchema("blog_post_schema");
            if (!getSchemaResult.IsSuccess)
            {
                var schemaXml = File.ReadAllText(blogPostSchemaFileName);
                var schema = new SearchSchema(blogPostSchemaName, schemaXml);
                var rslt = client.PutSearchSchema(schema);
                CheckResult(rslt);

                WaitForSearch();

                var idx = new SearchIndex("blog_posts", "blog_post_schema");
                rslt = client.PutSearchIndex(idx);
                CheckResult(rslt);
            }
        }

        [Test]
        public void StoreBlogPost()
        {
            var keywords = new HashSet<string> { "adorbs", "cheshire", "funny" };

            var post = new BlogPost(
                "This one is so lulz!",
                "Cat Stevens",
                "Please check out these cat pics!",
                keywords,
                DateTime.Now,
                true);

            var repo = new BlogPostRepository(client, "cat_pics_quarterly");
            string id = repo.Save(post);
            Assert.IsNotNullOrEmpty(id);
            Console.WriteLine("Blog post ID: {0}", id);
        }

        [Test]
        public void QueryBlogPostsKeywords()
        {
            StoreBlogPost();

            WaitForSearch();

            var searchRequest = new RiakSearchRequest("blog_posts", "keywords_set:funny");
            var rslt = client.Search(searchRequest);
            CheckResult(rslt);
            foreach (var doc in rslt.Value.Documents)
            {
                Console.WriteLine("Key: {0}, Content:\n{1}",
                    doc.Key, GetFields(doc));
            }
        }

        private static string GetFields(RiakSearchResultDocument doc)
        {
            var sb = new StringBuilder();

            foreach (var f in doc.Fields)
            {
                sb.AppendFormat("\t{0}: {1}\n", f.Key, f.Value);
            }

            return sb.ToString();
        }
    }
}