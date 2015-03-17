// <copyright file="BlogPostRepository.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;

    public class BlogPostRepository : Repository<BlogPost>
    {
        const string titleRegister = "title";
        const string authorRegister = "author";
        const string contentRegister = "content";
        const string keywordsSet = "keywords";
        const string datePostedRegister = "date";
        const string publishedFlag = "published";

        private static readonly SerializeObjectToByteArray<string> Serializer =
            s => Encoding.UTF8.GetBytes(s);

        private readonly string bucketName;

        public BlogPostRepository(IRiakClient client, string bucketName)
            : base(client)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }

            this.bucketName = bucketName;
        }

        public override string Save(BlogPost model)
        {
            var updates = new List<MapUpdate>();

            updates.Add(new MapUpdate
            {
                register_op = Serializer(model.Title),
                field = new MapField
                {
                    name = Serializer(titleRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            updates.Add(new MapUpdate
            {
                register_op = Serializer(model.Author),
                field = new MapField
                {
                    name = Serializer(authorRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            updates.Add(new MapUpdate
            {
                register_op = Serializer(model.Content),
                field = new MapField
                {
                    name = Serializer(contentRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            var keywordsSetOp = new SetOp();
            keywordsSetOp.adds.AddRange(model.Keywords.Select(kw => Serializer(kw)));

            updates.Add(new MapUpdate
            {
                set_op = keywordsSetOp,
                field = new MapField
                {
                    name = Serializer(keywordsSet),
                    type = MapField.MapFieldType.SET
                }
            });

            string datePostedSolrFormatted =
                model.DatePosted
                    .ToUniversalTime()
                    .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
            updates.Add(new MapUpdate
            {
                register_op = Serializer(datePostedSolrFormatted),
                field = new MapField
                {
                    name = Serializer(datePostedRegister),
                    type = MapField.MapFieldType.REGISTER
                }
            });

            updates.Add(new MapUpdate
            {
                flag_op = model.Published ?
                    MapUpdate.FlagOp.ENABLE : MapUpdate.FlagOp.DISABLE,
                field = new MapField
                {
                    name = Serializer(publishedFlag),
                    type = MapField.MapFieldType.FLAG
                }
            });

            var id = new RiakObjectId(BucketType, bucketName, null);

            var rslt = client.DtFetchMap(id);
            CheckResult(rslt.Result);

            rslt = client.DtUpdateMap(id, Serializer, rslt.Context, null, updates, null);
            CheckResult(rslt.Result);

            RiakObject obj = rslt.Result.Value;
            return obj.Key;
        }

        protected override string BucketType
        {
            get { return "cms"; }
        }
    }
}
