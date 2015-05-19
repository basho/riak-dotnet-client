// <copyright file="FetchCommandBuilder{TBuilder,TCommand,TOptions,TResponse}.cs" company="Basho Technologies, Inc.">
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
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the fetch command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for the fetch command.</typeparam>
    /// <typeparam name="TResponse">The type of the fetch command's response.</typeparam>
    public abstract class FetchCommandBuilder<TBuilder, TCommand, TOptions, TResponse>
        : CommandBuilder<TBuilder, TCommand, TOptions>
        where TBuilder : FetchCommandBuilder<TBuilder, TCommand, TOptions, TResponse>
        where TCommand : FetchCommand<TResponse>
        where TOptions : FetchCommandOptions
        where TResponse : Response
    {
        private Quorum r;
        private Quorum pr;

        private bool notFoundOK = false;
        private bool includeContext = true;
        private bool useBasicQuorum = false;

        public override TCommand Build()
        {
            Options = BuildOptions();
            Options.R = r;
            Options.PR = pr;

            Options.Timeout = timeout;

            Options.NotFoundOK = notFoundOK;
            Options.IncludeContext = includeContext;
            Options.UseBasicQuorum = useBasicQuorum;

            return (TCommand)Activator.CreateInstance(typeof(TCommand), Options);
        }

        public TBuilder WithR(Quorum r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("r", "r may not be null");
            }

            this.r = r;
            return (TBuilder)this;
        }

        public TBuilder WithPR(Quorum pr)
        {
            if (pr == null)
            {
                throw new ArgumentNullException("pr", "pr may not be null");
            }

            this.pr = pr;
            return (TBuilder)this;
        }

        public TBuilder WithNotFoundOK(bool notFoundOK)
        {
            this.notFoundOK = notFoundOK;
            return (TBuilder)this;
        }

        public TBuilder WithIncludeContext(bool includeContext)
        {
            this.includeContext = includeContext;
            return (TBuilder)this;
        }

        public TBuilder WithBasicQuorum(bool basicQuorum)
        {
            this.useBasicQuorum = basicQuorum;
            return (TBuilder)this;
        }
    }
}