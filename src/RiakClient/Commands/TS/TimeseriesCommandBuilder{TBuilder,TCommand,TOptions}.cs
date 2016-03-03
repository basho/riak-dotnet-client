namespace RiakClient.Commands.TS
{
    using System;

    /// <summary>
    /// Base class for all Riak command builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for this command.</typeparam>
    public abstract class TimeseriesCommandBuilder<TBuilder, TCommand, TOptions>
        : CommandBuilder<TBuilder, TCommand, TOptions>
        where TBuilder : TimeseriesCommandBuilder<TBuilder, TCommand, TOptions>
    {
        protected string table;

        public TimeseriesCommandBuilder()
        {
        }

        public TimeseriesCommandBuilder(TimeseriesCommandBuilder<TBuilder, TCommand, TOptions> source)
        {
            this.table = source.table;
            this.timeout = source.timeout;
        }

        public override TCommand Build()
        {
            Options = BuildOptions();
            PopulateOptions(Options);
            return (TCommand)Activator.CreateInstance(typeof(TCommand), Options);
        }

        public TBuilder WithTable(string table)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentNullException("table", "table may not be null, empty or whitespace");
            }

            this.table = table;
            return (TBuilder)this;
        }

        protected override TOptions BuildOptions()
        {
            return (TOptions)Activator.CreateInstance(typeof(TOptions), table);
        }

        protected abstract void PopulateOptions(TOptions options);
    }
}
