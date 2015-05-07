// <copyright file="BlogPostRepository.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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
    using System.Globalization;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Models;

    public class BlogPostRepository : Repository<BlogPost>
    {
        const string titleRegister = "title";
        const string authorRegister = "author";
        const string contentRegister = "content";
        const string keywordsSet = "keywords";
        const string datePostedRegister = "date";
        const string publishedFlag = "published";

        private readonly string bucket;

        public BlogPostRepository(IRiakClient client, string bucket)
            : base(client)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentNullException("bucket");
            }

            this.bucket = bucket;
        }

        public override string Save(BlogPost model)
        {
            var mapOp = new UpdateMap.MapOperation();

            mapOp.SetRegister(titleRegister, model.Title);
            mapOp.SetRegister(authorRegister, model.Author);
            mapOp.SetRegister(contentRegister, model.Content);
            mapOp.AddToSet(keywordsSet, model.Keywords);

            string datePostedSolrFormatted =
                model.DatePosted
                    .ToUniversalTime()
                    .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);

            mapOp.SetRegister(datePostedRegister, datePostedSolrFormatted);

            mapOp.SetFlag(publishedFlag, model.Published);

            // NB: no key so Riak will generate it
            var cmd = new UpdateMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(this.bucket)
                .WithMapOperation(mapOp)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            CheckResult(rslt);
            MapResponse response = cmd.Response;
            return response.Key;
        }

        protected override string BucketType
        {
            get { return "cms"; }
        }
    }
}
