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
        : KvCommandBuilder<TBuilder, TCommand, TOptions>
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

        public override IRCommand Build()
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
