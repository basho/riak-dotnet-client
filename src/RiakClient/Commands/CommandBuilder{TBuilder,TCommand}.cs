namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public abstract class CommandBuilder<TBuilder, TCommand> : IRCommandBuilder
        where TCommand : IRCommand
        where TBuilder : CommandBuilder<TBuilder, TCommand>
    {
        protected TimeSpan timeout;

        public CommandBuilder()
        {
        }

        public CommandBuilder(CommandBuilder<TBuilder, TCommand> source)
        {
            timeout = source.timeout;
        }

        public abstract IRCommand Build();

        public TBuilder WithTimeout(TimeSpan timeout)
        {
            if (timeout == default(TimeSpan))
            {
                throw new ArgumentException("Timeout must have non-default value.", "timeout");
            }

            this.timeout = timeout;
            return (TBuilder)this;
        }
    }
}
