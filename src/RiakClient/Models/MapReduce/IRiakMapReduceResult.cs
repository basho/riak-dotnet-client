// <copyright file="IRiakMapReduceResult.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.MapReduce
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an interface for mapreduce results.
    /// </summary>
    public interface IRiakMapReduceResult
    {
        /// <summary>
        /// The collection of <see cref="RiakMapReduceResultPhase"/> results, 
        /// one for each phase where the results of the phase were kept.
        /// </summary>
        IEnumerable<RiakMapReduceResultPhase> PhaseResults { get; }
    }
}
