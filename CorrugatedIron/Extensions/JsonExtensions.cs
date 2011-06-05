using System;
using CorrugatedIron.Containers;
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

        public static JsonWriter WriteNonNullProperty<T>(this JsonWriter writer, string name, T value)
            where T : class
        {
            if (value != null)
            {
                writer.WriteProperty(name, value);
            }
            return writer;
        }

        public static JsonWriter WriteEither<TLeft, TRight>(this JsonWriter writer, string name, Either<TLeft, TRight> either)
        {
            if (either != null)
            {
                if (either.IsLeft)
                {
                    writer.WriteProperty(name, either.Left);
                }
                else
                {
                    writer.WriteProperty(name, either.Right);
                }
            }
            return writer;
        }

        public static JsonWriter WriteNullableProperty<T>(this JsonWriter writer, string name, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                writer.WriteProperty(name, value.Value);
            }
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
