namespace CorrugatedIron.Models.MapReduce
{
    public class RiakReducePhase : RiakActionPhase
    {
        public override string PhaseType
        {
            get { return "reduce"; }
        }
    }
}
