// <copyright file="RiakOptions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A collection of common options for most Riak operations.
    /// </summary>
    /// <typeparam name="T">The concrete subclass type.</typeparam>
    [ComVisible(false)]
    public abstract class RiakOptions<T> where T : RiakOptions<T>
    {
        /// <summary>
        /// The number of replicas that must return before a delete is considered a succes.
        /// </summary>
        /// <value>
        /// The R value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum R { get; protected set; }

        /// <summary>
        /// The number of replicas that must respond before a write is considered a success.
        /// </summary>
        /// <value>The W value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum W { get; protected set; }

        /// <summary>
        /// Primary Read Quorum - the number of replicas that need to be available when retrieving the object.
        /// </summary>
        /// <value>
        /// The primary read quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum Pr { get; protected set; }

        /// <summary>
        /// Primary Write Quorum - the number of replicas need to be available when the write is attempted.
        /// </summary>
        /// <value>
        /// The primary write quorum. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum Pw { get; protected set; }

        /// <summary>
        /// Durable writes - the number of replicas that must commit to durable storage before returning a successful response.
        /// </summary>
        /// <value>
        /// The durable write value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.
        /// </value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum Dw { get; protected set; }

        /// <summary>
        /// The number of replicas that need to agree when retrieving the object.
        /// </summary>
        /// <value>The RW Value. Possible values include 'default', 'one', 'quorum', 'all', or any integer.</value>
        /// <remarks>Developers looking for an easy way to set this can look at <see cref="Quorum.WellKnown"/></remarks>
        public Quorum Rw { get; protected set; }

        /// <summary>
        /// The <see cref="RiakClient.Timout"/> period for an operation.
        /// </summary>
        /// <remarks>Developers can leave this unset by default.</remarks>
        public Timeout Timeout { get; protected set; }

        /// <summary>
        /// A fluent setter for the <see cref="R"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetR(Quorum value)
        {
            R = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="R"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetR(int value)
        {
            R = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="W"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetW(Quorum value)
        {
            W = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="W"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetW(int value)
        {
            W = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Pr"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetPr(Quorum value)
        {
            Pr = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Pr"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetPr(int value)
        {
            Pr = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Pw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetPw(Quorum value)
        {
            Pw = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Pw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetPw(int value)
        {
            Pw = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Dw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetDw(Quorum value)
        {
            Dw = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Dw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetDw(int value)
        {
            Dw = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Rw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetRw(Quorum value)
        {
            Rw = value;
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Rw"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetRw(int value)
        {
            Rw = new Quorum(value);
            return (T)this;
        }

        /// <summary>
        /// A fluent setter for the <see cref="Timeout"/> property.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public T SetTimeout(Timeout value)
        {
            Timeout = value;
            return (T)this;
        }
    }
}