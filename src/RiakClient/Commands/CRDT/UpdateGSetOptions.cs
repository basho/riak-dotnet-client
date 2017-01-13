// <copyright file="UpdateGSetOptions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands.CRDT
{
    using System;
    using System.Collections.Generic;
    using Extensions;

    /// <summary>
    /// Represents options for a <see cref="UpdateGSet"/> operation.
    /// </summary>
    /// <inheritdoc />
    public class UpdateGSetOptions : UpdateCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGSetOptions"/> class.
        /// </summary>
        /// <inheritdoc />
        public UpdateGSetOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }

        /// <summary>
        /// The <see cref="UpdateGSet"/> additions.
        /// </summary>
        /// <value>The values to add via the <see cref="UpdateGSet"/> command.</value>
        public ISet<byte[]> Additions
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="UpdateGSet"/> additions, as UTF8-encoded strings.
        /// </summary>
        /// <value>The values to add via the <see cref="UpdateGSet"/> command.</value>
        public ISet<string> AdditionsAsStrings
        {
            get { return Additions.GetUTF8Strings(); }
            set { Additions = value.GetUTF8Bytes(); }
        }

        protected override bool GetHasRemoves()
        {
            return false;
        }
    }
}