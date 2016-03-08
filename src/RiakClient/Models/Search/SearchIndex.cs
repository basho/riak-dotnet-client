// <copyright file="SearchIndex.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

    /// <summary>
    /// Represents a Lucene search index.
    /// </summary>
    public class SearchIndex
    {
        private readonly string name;
        private readonly string schemaName;
        private readonly NVal nval;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndex"/> class.
        /// </summary>
        /// <param name="indexName">The name of the index.</param>
        /// <remarks>
        /// Uses the default schema name (<see cref="RiakConstants.Defaults.YokozunaIndex.DefaultSchemaName"/>), 
        /// and NVal value (<see cref="RiakConstants.Defaults.YokozunaIndex.NVal"/>).
        /// </remarks>
        public SearchIndex(string indexName)
            : this(indexName, RiakConstants.Defaults.YokozunaIndex.DefaultSchemaName, new NVal(RiakConstants.Defaults.YokozunaIndex.NVal))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndex"/> class.
        /// </summary>
        /// <param name="indexName">The name of the index.</param>
        /// <param name="schemaName">The name of the schema for the index.</param>
        /// <remarks>
        /// Uses the provided schema name and default NVal value (<see cref="RiakConstants.Defaults.YokozunaIndex.NVal"/>).
        /// </remarks>
        public SearchIndex(string indexName, string schemaName)
            : this(indexName, schemaName, new NVal(RiakConstants.Defaults.YokozunaIndex.NVal))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndex"/> class.
        /// </summary>
        /// <param name="indexName">The name of the index.</param>
        /// <param name="schemaName">The name of the schema for the index.</param>
        /// <param name="nval">The <see cref="NVal"/> value for storing index entries.</param>
        /// <exception cref="ArgumentException"><paramref name="indexName"/> cannot be null, zero length, or whitespace</exception>
        /// <exception cref="ArgumentException"><paramref name="schemaName"/> cannot be null, zero length, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="nval"/> must not be null.</exception>
        public SearchIndex(string indexName, string schemaName, NVal nval)
        {
            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException("Index Name cannot be null, zero length, or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(schemaName))
            {
                throw new ArgumentException("Schema Name cannot be null, zero length, or whitespace.");
            }

            if (nval == null)
            {
                throw new ArgumentOutOfRangeException("nval", "nval must not be null.");
            }

            this.name = indexName;
            this.nval = nval;
            this.schemaName = schemaName;
        }

        internal SearchIndex(RpbYokozunaIndex index)
        {
            this.name = index.name.FromRiakString();
            this.schemaName = index.schema.FromRiakString();
            this.nval = new NVal(index.n_val);
        }

        /// <summary>
        /// The name of the index.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The name of the index's schema.
        /// </summary>
        public string SchemaName
        {
            get { return schemaName; }
        }

        /// <summary>
        /// The <see cref="NVal"/> value for storing index entries.
        /// I.e. - the number of copies to store the index to.
        /// </summary>
        public NVal NVal
        {
            get { return nval; }
        }

        /// <summary>
        /// The <see cref="Timeout"/> value for storing index entries.
        /// </summary>
        public Timeout? Timeout
        {
            get;
            set;
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
