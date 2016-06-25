namespace RiakClient.Models.MapReduce.Phases
{
    using Models.MapReduce.Languages;

    internal class RiakMapPhase<TLanguage> : RiakActionPhase<TLanguage>
        where TLanguage : IRiakPhaseLanguage, new()
    {
        /// <inheritdoc/>
        public override string PhaseType
        {
            get { return "map"; }
        }
    }
}
