namespace RiakClient.Converters
{
    using System;
    using Models;
    using Newtonsoft.Json;

    /*
     * TODO 3.0: FUTURE - Figure out if this is still needed
     */
    internal class RiakObjectIdConverter : JsonConverter
    {
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int pos = 0;
            string bucket = null;
            string key = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.PropertyName)
                {
                    if (pos == 0)
                    {
                        bucket = reader.Value.ToString();
                    }

                    if (pos == 1)
                    {
                        key = reader.Value.ToString();
                    }
                }

                if (pos > 1)
                {
                    break;
                }

                pos++;
            }

            return new RiakObjectId(bucket, key);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
