namespace RiakClient.Models.MapReduce.Phases
{
    using Models.MapReduce.Languages;
    using Newtonsoft.Json;

    internal abstract class RiakActionPhase<TLanguage> : RiakPhase
        where TLanguage : IRiakPhaseLanguage, new()
    {
        private readonly TLanguage language;
        private object argument;

        protected RiakActionPhase()
        {
            this.language = new TLanguage();
        }

        /// <summary>
        /// The language type of the phase.
        /// (<see cref="RiakPhaseLanguageErlang"/> or <see cref="RiakPhaseLanguageJavascript"/>).
        /// </summary>
        public TLanguage Language
        {
            get { return language; }
        }

        /// <summary>
        /// The optional arguments to pass onto the phase function.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="argument"/> parameter.</typeparam>
        /// <param name="argument">The argument to pass on.</param>
        public void Argument<T>(T argument)
        {
            this.argument = argument;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            Language.WriteJson(writer);

            if (argument != null)
            {
                var json = JsonConvert.SerializeObject(argument);
                writer.WritePropertyName("arg");
                writer.WriteRawValue(json);
            }
        }
    }
}
