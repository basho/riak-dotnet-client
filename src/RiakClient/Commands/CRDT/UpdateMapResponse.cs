// <copyright file="UpdateMapResponse.cs" company="Basho Technologies, Inc.">
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
    using Exceptions;
    using Messages;

    /// <summary>
    /// Response to a <see cref="FetchMap"/> or <see cref="UpdateMap"/> command.
    /// </summary>
    public class UpdateMapResponse
    {
        private static readonly UpdateMapResponse NotFoundResponseField;
        private readonly bool notFound;

        static UpdateMapResponse()
        {
            NotFoundResponseField = new UpdateMapResponse(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMapResponse"/> class.
        /// </summary>
        /// <param name="fetchResp">The PB message from which to construct this <see cref="UpdateMapResponse"/></param>
        public UpdateMapResponse(DtUpdateResp fetchResp)
        {
        }

        private UpdateMapResponse(bool notFound)
        {
            this.notFound = notFound;
        }

        public static UpdateMapResponse NotFoundResponse
        {
            get { return NotFoundResponseField; }
        }

        public bool NotFound
        {
            get { return notFound; }
        }
    }
}