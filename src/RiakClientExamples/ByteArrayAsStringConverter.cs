namespace RiakClientExamples
{
    using System;
    using System.Text;
    using Newtonsoft.Json;

    internal class ByteArrayAsStringConverter : JsonConverter
    {
        private static readonly Type byteArrayType = typeof(byte[]);

        public override bool CanConvert(Type objectType)
        {
            return objectType == byteArrayType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (object.ReferenceEquals(value, null))
            {
                writer.WriteNull();
                return;
            }

            string data = Encoding.UTF8.GetString((byte[])value);
            writer.WriteValue(data);
        }
    }
}