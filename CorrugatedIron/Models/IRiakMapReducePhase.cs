namespace CorrugatedIron.Models
{
    public interface IRiakMapReducePhase
    {
        string MapReducePhaseType { get; set; }
        bool Keep { get; set; }
        string ToJsonString();
    }
}