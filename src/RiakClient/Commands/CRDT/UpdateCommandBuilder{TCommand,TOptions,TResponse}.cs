// <copyright file="UpdateCommandBuilder{TCommand,TOptions,TResponse}.cs" company="Basho Technologies, Inc.">
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
    using Util;

    /// <summary>
    /// Builds a fetch command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the update command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for the update command.</typeparam>
    /// <typeparam name="TResponse">The type of the update command's response.</typeparam>
    public abstract class UpdateCommandBuilder<TCommand, TOptions, TResponse>
        where TCommand : UpdateCommand<TResponse>
        where TOptions : UpdateCommandOptions
        where TResponse : Response, new()
    {
        private readonly CommandBuilderHelper helper = new CommandBuilderHelper();

        private Quorum w;
        private Quorum pw;
        private Quorum dw;

        private bool returnBody = true; // NB: default to true

        private bool includeContext = true; // NB: default to true
        private byte[] context;

        public UpdateCommandBuilder()
        {
        }

        public UpdateCommandBuilder(UpdateCommandBuilder<TCommand, TOptions, TResponse> source)
        {
            this.helper = source.helper;

            this.w = source.w;
            this.pw = source.pw;
            this.dw = source.dw;

            this.returnBody = source.returnBody;

            this.includeContext = source.includeContext;
            this.context = source.context;
        }

        public TOptions Options
        {
            get;
            private set;
        }

        public TCommand Build()
        {
            this.Options = (TOptions)Activator.CreateInstance(typeof(TOptions), helper.BucketType, helper.Bucket, helper.Key);

            Options.W = w;
            Options.PW = pw;
            Options.DW = dw;

            Options.ReturnBody = returnBody;

            Options.Timeout = helper.Timeout;

            Options.IncludeContext = includeContext;
            Options.Context = context;

            PopulateOptions(Options);

            return (TCommand)Activator.CreateInstance(typeof(TCommand), Options);
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithBucketType(string bucketType)
        {
            helper.WithBucketType(bucketType);
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithBucket(string bucket)
        {
            helper.WithBucket(bucket);
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithKey(string key)
        {
            helper.WithKey(key);
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithTimeout(TimeSpan timeout)
        {
            helper.WithTimeout(timeout);
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithContext(byte[] context)
        {
            this.context = context;
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithW(Quorum w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w", "w may not be null");
            }

            this.w = w;
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithPW(Quorum pw)
        {
            if (pw == null)
            {
                throw new ArgumentNullException("pw", "pw may not be null");
            }

            this.pw = pw;
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithDW(Quorum dw)
        {
            if (dw == null)
            {
                throw new ArgumentNullException("dw", "dw may not be null");
            }

            this.dw = dw;
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithReturnBody(bool returnBody)
        {
            this.returnBody = returnBody;
            return this;
        }

        public UpdateCommandBuilder<TCommand, TOptions, TResponse> WithIncludeContext(bool includeContext)
        {
            this.includeContext = includeContext;
            return this;
        }

        protected abstract void PopulateOptions(TOptions options);
    }
}