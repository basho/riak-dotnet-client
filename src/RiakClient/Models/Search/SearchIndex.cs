// <copyright file="SearchIndex.cs" company="Basho Technologies, Inc.">
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

    public class SearchIndex
    {
        private readonly string name;
        private readonly string schemaName;
        private readonly NVal nval;

        public SearchIndex(string name)
            : this(name, RiakConstants.Defaults.YokozunaIndex.IndexName, new NVal(RiakConstants.Defaults.YokozunaIndex.NVal))
        {
        }

        public SearchIndex(string name, string schemaName, NVal nval)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Index Name cannot be null, zero length, or whitespace");
            }

            if (string.IsNullOrWhiteSpace(schemaName))
            {
                throw new ArgumentException("Schema Name cannot be null, zero length, or whitespace");
            }

            if (nval == null)
            {
                throw new ArgumentNullException("nval must be greater than 0");
            }

            this.name = name;
            this.nval = nval;
            this.schemaName = schemaName;
        }

        internal SearchIndex(RpbYokozunaIndex index)
        {
            this.name = index.name.FromRiakString();
            this.schemaName = index.schema.FromRiakString();
            this.nval = new NVal(index.n_val);
        }

        public string Name
        {
            get { return name; }
        }

        public string SchemaName
        {
            get { return schemaName; }
        }

        public NVal NVal
        {
            get { return nval; }
        }

        internal RpbYokozunaIndex ToMessage()
        {
            return new RpbYokozunaIndex
            {
                name = name.ToRiakString(),
                schema = schemaName.ToRiakString(),
                n_val = nval
            };
        }
    }
}