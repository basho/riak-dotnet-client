// <copyright file="FetchMapResponse.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using Exceptions;
    using Messages;

    /// <summary>
    /// Response to a <see cref="FetchMap"/> or <see cref="FetchMap"/> command.
    /// </summary>
    public class FetchMapResponse
    {
        private static readonly FetchMapResponse NotFoundResponseField;
        private readonly bool notFound;
        private readonly byte[] context;
        private readonly Map map = new Map();

        static FetchMapResponse()
        {
            NotFoundResponseField = new FetchMapResponse(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchMapResponse"/> class.
        /// </summary>
        /// <param name="fetchResp">The PB message from which to construct this <see cref="FetchMapResponse"/></param>
        public FetchMapResponse(DtFetchResp fetchResp)
        {
            if (fetchResp.type != DtFetchResp.DataType.MAP)
            {
                throw new RiakException(
                    string.Format("Requested map, received {0}", fetchResp.type));
            }

            this.context = fetchResp.context;

            ParsePbResponse(fetchResp.value.map_value);
        }

        private FetchMapResponse(bool notFound)
        {
            this.notFound = notFound;
        }

        public static FetchMapResponse NotFoundResponse
        {
            get { return NotFoundResponseField; }
        }

        public bool NotFound
        {
            get { return notFound; }
        }

        public byte[] Context
        {
            get { return context; }
        }

        public Map Map
        {
            get { return map; }
        }

        private void ParsePbResponse(IEnumerable<MapEntry> mapEntries)
        {
        }
    }
}
