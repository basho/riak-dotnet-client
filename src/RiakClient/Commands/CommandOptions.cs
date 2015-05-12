// <copyright file="CommandOptions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command options.
    /// </summary>
    public abstract class CommandOptions : IEquatable<CommandOptions>
    {
        private readonly RiakString bucketType;
        private readonly RiakString bucket;
        private readonly RiakString key;

        private Timeout timeout = CommandDefaults.Timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="bucketType">The bucket type in Riak. Required.</param>
        /// <param name="bucket">The bucket in Riak. Required.</param>
        /// <param name="key">The key in Riak.</param>
        /// <param name="keyIsRequired">If <b>true</b> and no key given, an exception is thrown.</param>
        /// <param name="timeout">The command timeout in Riak. Default is <b>60 seconds</b></param>
        public CommandOptions(
            string bucketType,
            string bucket,
            string key,
            bool keyIsRequired,
            Timeout timeout = null)
        {
            if (string.IsNullOrEmpty(bucketType))
            {
                throw new ArgumentNullException("bucketType");
            }
            else
            {
                this.bucketType = bucketType;
            }

            if (string.IsNullOrEmpty(bucket))
            {
                throw new ArgumentNullException("bucket");
            }
            else
            {
                this.bucket = bucket;
            }

            if (keyIsRequired && string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            else
            {
                this.key = key;
            }

            if (timeout != null)
            {
                this.timeout = timeout;
            }
        }

        /// <summary>
        /// The bucket type
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket type.</value>
        public RiakString BucketType
        {
            get { return bucketType; }
        }

        /// <summary>
        /// The bucket
        /// </summary>
        /// <value>A <see cref="RiakString"/> representing the bucket.</value>
        public RiakString Bucket
        {
            get { return bucket; }
        }

        /// <summary>
        /// The key
        /// </summary>
        /// <value>The <see cref="RiakString"/> representing the key.</value>
        public RiakString Key
        {
            get { return key; }
        }

        /// <summary>
        /// The timeout for this command.
        /// </summary>
        public Timeout Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                if (value == null)
                {
                    timeout = CommandDefaults.Timeout;
                }
                else
                {
                    timeout = value;
                }
            }
        }

        public bool Equals(CommandOptions other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandOptions);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// Uses a combination of the public properties to generate a unique hash code.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = bucketType.GetHashCode();
                result = (result * 397) ^ bucket.GetHashCode();
                result = (result * 397) ^ (key != null ? key.GetHashCode() : 0);
                result = (result * 397) ^ (timeout != null ? timeout.GetHashCode() : 0);
                return result;
            }
        }
    }
}