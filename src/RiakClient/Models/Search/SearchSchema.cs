// <copyright file="SearchSchema.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClient.Models.Search
{
    using System;
    using Extensions;
    using Messages;

    public class SearchSchema
    {
        private readonly string name;
        private readonly string content;

        public SearchSchema(string name, string content)
        {
            this.name = name;
            this.content = content;
        }

        internal SearchSchema(RpbYokozunaSchema schema)
        {
            this.name = schema.name.FromRiakString();
            this.content = schema.content.FromRiakString();
        }

        public string Name
        {
            get { return name; }
        }

        public string Content
        {
            get { return content; }
        }

        internal RpbYokozunaSchema ToMessage()
        {
            return new RpbYokozunaSchema
            {
                name = this.name.ToRiakString(),
                content = this.content.ToRiakString()
            };
        }
    }
}
