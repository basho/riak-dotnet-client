// <copyright file="CommandBuilder{TBuilder,TCommand,TOptions}.cs" company="Basho Technologies, Inc.">
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
    /// Base class for all Riak command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    public abstract class CommandBuilder<TBuilder, TCommand, TOptions>
        where TBuilder : CommandBuilder<TBuilder, TCommand, TOptions>
    {
        protected string bucketType;
        protected string bucket;
        protected string key;
        protected Timeout timeout = CommandDefaults.Timeout;

        public CommandBuilder()
        {
        }

        public CommandBuilder(CommandBuilder<TBuilder, TCommand, TOptions> source)
        {
            this.bucketType = source.bucketType;
            this.bucket = source.bucket;
            this.key = source.key;
            this.timeout = source.timeout;
        }

        public TOptions Options
        {
            get;
            protected set;
        }

        public abstract TCommand Build();

        public TBuilder WithBucketType(string bucketType)
        {
            if (string.IsNullOrWhiteSpace(bucketType))
            {
                throw new ArgumentNullException("bucketType", "bucketType may not be null, empty or whitespace");
            }

            this.bucketType = bucketType;
            return (TBuilder)this;
        }

        public TBuilder WithBucket(string bucket)
        {
            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new ArgumentNullException("bucket", "bucket may not be null, empty or whitespace");
            }

            this.bucket = bucket;
            return (TBuilder)this;
        }

        public TBuilder WithKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "key may not be null, empty or whitespace");
            }

            this.key = key;
            return (TBuilder)this;
        }

        public TBuilder WithTimeout(Timeout timeout)
        {
            this.timeout = timeout;
            return (TBuilder)this;
        }

        public TBuilder WithTimeout(TimeSpan timeout)
        {
            if (timeout == default(TimeSpan))
            {
                this.timeout = CommandDefaults.Timeout;
            }
            else
            {
                this.timeout = new Timeout(timeout);
            }

            return (TBuilder)this;
        }
    }
}
