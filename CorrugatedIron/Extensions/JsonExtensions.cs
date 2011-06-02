using Newtonsoft.Json;

namespace CorrugatedIron.Extensions
{
    public static class JsonExtensions
    {
        public static JsonWriter WriteProperty<T>(this JsonWriter writer, string name, T value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
            return writer;
        }

        public static JsonWriter WriteSpecifiedProperty(this JsonWriter writer, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                writer.WriteProperty(name, value);
            }
            return writer;
        }
    }
}
