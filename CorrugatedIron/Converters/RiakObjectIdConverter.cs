using System;
using CorrugatedIron.Models;
using Newtonsoft.Json;

namespace CorrugatedIron.Converters
{
    public class RiakObjectIdConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            var pos = 0;
            var objectIdParts = new string[2];

            while(reader.Read())
            {
                if(pos < 2 && reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.PropertyName)
                {
                    objectIdParts[pos] = reader.Value.ToString();
                    pos++;
                }
            }

            return new RiakObjectId(objectIdParts);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}