// <copyright file="RiakInvalidDataException.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Represents an error that occurs when an invalid message code is read from a Riak response.
    /// </summary>
    [Serializable]
    public class RiakInvalidDataException : RiakException
    {
        private readonly byte messageCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        public RiakInvalidDataException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RiakInvalidDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RiakInvalidDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakInvalidDataException"/> class.
        /// </summary>
        /// <param name="messageCode">The invalid message code that was read from the Riak response.</param>
        public RiakInvalidDataException(byte messageCode)
            : this(string.Format("Unexpected message code: {0}", messageCode))
        {
            this.messageCode = messageCode;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RiakInvalidDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.messageCode = info.GetByte("MessageCode");
        }

        /// <summary>
        /// The invalid message code that was read from the Riak response.
        /// </summary>
        public byte MessageCode
        {
            get
            {
                return messageCode;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="info"/>' cannot be null. </exception>
        /// <exception cref="SerializationException">A value has already been associated with '<paramref name="name" />'.</exception>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("MessageCode", messageCode);

            base.GetObjectData(info, context);
        }
    }
}
