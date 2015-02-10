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

    [Serializable]
    public class RiakInvalidDataException : RiakException
    {
        private readonly byte messageCode;

        public RiakInvalidDataException()
        {
        }

        public RiakInvalidDataException(string message)
            : base(message)
        {
        }

        public RiakInvalidDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

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

        public byte MessageCode
        {
            get
            {
                return messageCode;
            }
        }

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
