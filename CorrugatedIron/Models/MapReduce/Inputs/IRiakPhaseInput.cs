using Newtonsoft.Json;

namespace CorrugatedIron.Models.MapReduce.Inputs
{
    public interface IRiakPhaseInput
    {
        JsonWriter WriteJson(JsonWriter writer);
    }
}
