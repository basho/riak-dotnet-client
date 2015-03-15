// <copyright file="DocumentStore.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples.Dev.Search
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using NUnit.Framework;
    using RiakClient;
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
                var req = WebRequest.CreateHttp(blogPostSchema);
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

            var schemaXml = File.ReadAllText(blogPostSchemaFileName);
            var schema = new SearchSchema(blogPostSchemaName, schemaXml);
            var rslt = client.PutSearchSchema(schema);
            CheckResult(rslt);

            Thread.Sleep(1250);

            var idx = new SearchIndex("blog_posts", "blog_post_schema");
            rslt = client.PutSearchIndex(idx);
            CheckResult(rslt);
        }

        [Test]
        public void CreateSearchIndex()
        {
            Assert.True(1 == 1);
        }
    }
}
