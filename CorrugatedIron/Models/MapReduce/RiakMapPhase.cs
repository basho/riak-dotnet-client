namespace CorrugatedIron.Models.MapReduce
{
    public class RiakMapPhase : RiakActionPhase
    {
        public override string PhaseType
        {
            get { return "map"; }
        }

    }
}
