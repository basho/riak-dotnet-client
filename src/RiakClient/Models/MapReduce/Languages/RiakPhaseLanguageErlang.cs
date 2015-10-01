namespace RiakClient.Models.MapReduce.Languages
{
    using Extensions;
    using Newtonsoft.Json;

    internal class RiakPhaseLanguageErlang : IRiakPhaseLanguage
    {
        private string module;
        private string function;

        /// <summary>
        /// The Erlang Module:Function to execute for this phase.
        /// </summary>
        /// <param name="module">The module containing the <paramref name="function"/> to execute.</param>
        /// <param name="function">The function to execute for this phase.</param>
        public void ModFun(string module, string function)
        {
            this.module = module;
            this.function = function;
        }

        /// <inheritdoc/>
        public void WriteJson(JsonWriter writer)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(module), "Module must be set");
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(function), "Function must be set");

            writer.WriteSpecifiedProperty("language", RiakConstants.MapReduceLanguage.Erlang)
                .WriteSpecifiedProperty("module", module)
                .WriteSpecifiedProperty("function", function);
        }
    }
}
