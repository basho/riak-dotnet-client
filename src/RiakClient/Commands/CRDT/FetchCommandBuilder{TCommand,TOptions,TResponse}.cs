// <copyright file="FetchCommandBuilder{TCommand,TOptions,TResponse}.cs" company="Basho Technologies, Inc.">
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
    using System;

    /// <summary>
    /// Builds a fetch command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the fetch command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for the fetch command.</typeparam>
    /// <typeparam name="TResponse">The type of the fetch command's response.</typeparam>
    public abstract class FetchCommandBuilder<TCommand, TOptions, TResponse>
        where TCommand : FetchCommand<TResponse>
        where TOptions : FetchCommandOptions
        where TResponse : Response
    {
        private readonly CommandBuilderHelper helper = new CommandBuilderHelper();

        private Quorum r;
        private Quorum pr;

        private bool notFoundOK = false;
        private bool includeContext = true;
        private bool useBasicQuorum = false;

        public TCommand Build()
        {
            TOptions options = (TOptions)Activator.CreateInstance(typeof(TOptions), helper.BucketType, helper.Bucket, helper.Key);
            options.R = r;
            options.PR = pr;

            options.Timeout = helper.Timeout;
            options.NotFoundOK = notFoundOK;
            options.IncludeContext = includeContext;
            options.UseBasicQuorum = useBasicQuorum;

            return (TCommand)Activator.CreateInstance(typeof(TCommand), options);
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithBucketType(string bucketType)
        {
            helper.WithBucketType(bucketType);
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithBucket(string bucket)
        {
            helper.WithBucket(bucket);
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithKey(string key)
        {
            helper.WithKey(key);
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithTimeout(TimeSpan timeout)
        {
            helper.WithTimeout(timeout);
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithR(Quorum r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r", "r may not be null");
            }

            this.r = r;
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithPR(Quorum pr)
        {
            if (pr == null)
            {
                throw new ArgumentNullException("pr", "pr may not be null");
            }

            this.pr = pr;
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithNotFoundOK(bool notFoundOK)
        {
            this.notFoundOK = notFoundOK;
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithIncludeContext(bool includeContext)
        {
            this.includeContext = includeContext;
            return this;
        }

        public FetchCommandBuilder<TCommand, TOptions, TResponse> WithBasicQuorum(bool basicQuorum)
        {
            this.useBasicQuorum = basicQuorum;
            return this;
        }
    }
}