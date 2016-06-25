namespace RiakClient.Commands
{
    /// <summary>
    /// Base class for all Riak command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    public abstract class CommandBuilder<TBuilder, TCommand, TOptions> : CommandBuilder<TBuilder, TCommand>
        where TBuilder : CommandBuilder<TBuilder, TCommand, TOptions>
        where TCommand : IRCommand
        where TOptions : CommandOptions
    {
        public CommandBuilder()
        {
        }

        public CommandBuilder(CommandBuilder<TBuilder, TCommand, TOptions> source)
            : base(source)
        {
        }

        public TOptions Options
        {
            get;
            protected set;
        }

        protected abstract TOptions BuildOptions();
    }
}
