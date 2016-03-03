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
        protected Timeout timeout = CommandDefaults.Timeout;

        public CommandBuilder()
        {
        }

        public CommandBuilder(CommandBuilder<TBuilder, TCommand, TOptions> source)
        {
            this.timeout = source.timeout;
        }

        public TOptions Options
        {
            get;
            protected set;
        }

        public abstract TCommand Build();

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

        protected abstract TOptions BuildOptions();
    }
}
