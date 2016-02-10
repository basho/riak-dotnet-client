namespace RiakClient.Models.MapReduce.Inputs
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an Erlang Module:Function(Arg) mapreduce input.
    /// </summary>
    public class RiakModuleFunctionArgInput : RiakPhaseInput
    {
        private readonly string[] arg;
        private readonly string function;
        private readonly string module;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakModuleFunctionArgInput"/> class.
        /// </summary>
        /// <param name="module">The Erlang module containing the <paramref name="function"/> to run.</param>
        /// <param name="function">The Erlang function to run, whose results will be used as inputs for the mapreduce job.</param>
        /// <param name="arg">Any arguments to pass to <paramref name="function"/>.</param>
        public RiakModuleFunctionArgInput(string module, string function, string[] arg)
        {
            this.module = module;
            this.function = function;
            this.arg = arg;
        }

        /// <summary>
        /// The Erlang module containing the <see cref="Function"/> to run.
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// The Erlang function to run, whose results will be used as inputs for the mapreduce job.
        /// </summary>
        public string Function
        {
            get { return function; }
        }

        /// <summary>
        /// Any arguments to pass to <see name="Function"/>.
        /// </summary>
        public string[] Arg
        {
            get { return arg; }
        }

        /// <inheritdoc/>
        public override JsonWriter WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("inputs");
            writer.WriteStartObject();

            writer.WritePropertyName("module");
            writer.WriteValue(Module);

            writer.WritePropertyName("function");
            writer.WriteValue(Function);

            writer.WritePropertyName("arg");

            writer.WriteStartArray();

            foreach (string arg in Arg)
            {
                writer.WriteValue(arg);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();

            return writer;
        }
    }
}
