// <copyright file="UpdateMapOptions.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Represents options for a <see cref="UpdateMap"/> operation.
    /// </summary>
    /// <inheritdoc />
    public class UpdateMapOptions : UpdateCommandOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMapOptions"/> class.
        /// </summary>
        /// <inheritdoc />
        public UpdateMapOptions(string bucketType, string bucket, string key)
            : base(bucketType, bucket, key)
        {
        }

        /// <summary>
        /// The <see cref="UpdateMap.MapOperation"/>
        /// </summary>
        /// <value>The <see cref="UpdateMap.MapOperation"/> to be executed by the <see cref="UpdateMap"/> command.</value>
        public UpdateMap.MapOperation Op
        {
            get;
            set;
        }

        protected override bool GetHasRemoves()
        {
            return Op.HasRemoves;
        }
    }
}