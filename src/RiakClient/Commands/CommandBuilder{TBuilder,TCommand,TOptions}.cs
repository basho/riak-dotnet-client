namespace RiakClient.Commands
{
    using System;

    /// <summary>
    /// Base class for all Riak command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    public abstract class CommandBuilder<TBuilder, TCommand, TOptions> : CommandBuilder<TBuilder, TCommand>
        where TCommand : IRCommand
        where TBuilder : CommandBuilder<TBuilder, TCommand, TOptions>
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

        protected TOptions BuildOptions()
        {
            return (TOptions)Activator.CreateInstance(typeof(TOptions), bucketType, bucket, key);
        }
    }
}
