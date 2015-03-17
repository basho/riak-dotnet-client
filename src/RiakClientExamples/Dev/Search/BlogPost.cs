// <copyright file="BlogPost.cs" company="Basho Technologies, Inc.">
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

    public class BlogPost : IModel
    {
        private readonly string title;
        private string author;
        private string content;
        private ISet<string> keywords;
        private DateTime datePosted;
        private Boolean published;

        public BlogPost(
            string title,
            string author,
            string content,
            ISet<string> keywords,
            DateTime datePosted,
            bool published)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException("title", "title is required");
            }
            this.title = title;

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentNullException("author", "author is required");
            }
            this.author = author;

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException("content", "content is required");
            }
            this.content = content;

            this.keywords = keywords;
            this.datePosted = datePosted;
            this.published = published;
        }

        public string ID
        {
            get { return CreateSlug(); }
        }

        public string Title
        {
            get { return title; }
        }

        public string Author
        {
            get { return author; }
        }

        public string Content
        {
            get { return content; }
        }

        public IEnumerable<string> Keywords
        {
            get { return keywords; }
        }

        public DateTime DatePosted
        {
            get { return datePosted; }
        }

        public bool Published
        {
            get { return published; }
        }

        private string CreateSlug()
        {
            return "TODO";
        }
    }
}
