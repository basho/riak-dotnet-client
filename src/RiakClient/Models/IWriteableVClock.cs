// <copyright file="IWriteableVClock.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    /// <summary>
    /// <para>Implements a writeable vector clock interface. Callers must explictly use the
    /// IWriteableVClock interface to set the vector clock value. This is by design and
    /// is implemented in an attempt to prevent developers new to Riak from causing themselves
    /// more pain. This trade off should present developers with a reliable way to explicitly
    /// drop down to mucking about with vector clocks - it becomes apparent to a casual 
    /// observer that something out of the ordinary is happening.</para>
    /// <para>A better understanding of the usefulness of vector clocks can be found in 
    /// John Daily's Understanding Riak’s Configurable Behaviors: Part 2
    /// (http://basho.com/riaks-config-behaviors-part-2/).
    /// </para>
    /// </summary>
    public interface IWriteableVClock
    {
        void SetVClock(byte[] vclock);
    }
}
