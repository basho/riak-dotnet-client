namespace RiakClient.Models.MapReduce.Languages
{
    using Newtonsoft.Json;

    internal interface IRiakPhaseLanguage
    {
        /// <summary>
        /// Serialize the phase to JSON and write it using the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        void WriteJson(JsonWriter writer);
    }
}
