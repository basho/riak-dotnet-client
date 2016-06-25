namespace RiakClient.Commands.CRDT
{
    using System;

    /// <summary>
    /// Builds a fetch command.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder. Allows chaining.</typeparam>
    /// <typeparam name="TCommand">The type of the update command.</typeparam>
    /// <typeparam name="TOptions">The type of the options for the update command.</typeparam>
    /// <typeparam name="TResponse">The type of the update command's response.</typeparam>
    public abstract class UpdateCommandBuilder<TBuilder, TCommand, TOptions, TResponse>
        : KvCommandBuilder<TBuilder, TCommand, TOptions>
        where TBuilder : UpdateCommandBuilder<TBuilder, TCommand, TOptions, TResponse>
        where TCommand : UpdateCommand<TResponse>
        where TOptions : UpdateCommandOptions
        where TResponse : Response, new()
    {
        private Quorum w;
        private Quorum pw;
        private Quorum dw;

        private bool returnBody = true; // NB: default to true

        private bool includeContext = true; // NB: default to true
        private byte[] context;

        public UpdateCommandBuilder()
        {
        }

        public UpdateCommandBuilder(UpdateCommandBuilder<TBuilder, TCommand, TOptions, TResponse> source)
            : base(source)
        {
            this.w = source.w;
            this.pw = source.pw;
            this.dw = source.dw;

            this.returnBody = source.returnBody;

            this.includeContext = source.includeContext;
            this.context = source.context;
        }

        public override IRCommand Build()
        {
            Options = BuildOptions();

            Options.W = w;
            Options.PW = pw;
            Options.DW = dw;

            Options.ReturnBody = returnBody;

            Options.Timeout = timeout;

            Options.IncludeContext = includeContext;
            Options.Context = context;

            PopulateOptions(Options);

            return (TCommand)Activator.CreateInstance(typeof(TCommand), Options);
        }

        public TBuilder WithContext(byte[] context)
        {
            this.context = context;
            return (TBuilder)this;
        }

        public TBuilder WithW(Quorum w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w", "w may not be null");
            }

            this.w = w;
            return (TBuilder)this;
        }

        public TBuilder WithPW(Quorum pw)
        {
            if (pw == null)
            {
                throw new ArgumentNullException("pw", "pw may not be null");
            }

            this.pw = pw;
            return (TBuilder)this;
        }

        public TBuilder WithDW(Quorum dw)
        {
            if (dw == null)
            {
                throw new ArgumentNullException("dw", "dw may not be null");
            }

            this.dw = dw;
            return (TBuilder)this;
        }

        public TBuilder WithReturnBody(bool returnBody)
        {
            this.returnBody = returnBody;
            return (TBuilder)this;
        }

        public TBuilder WithIncludeContext(bool includeContext)
        {
            this.includeContext = includeContext;
            return (TBuilder)this;
        }

        protected abstract void PopulateOptions(TOptions options);
    }
}
