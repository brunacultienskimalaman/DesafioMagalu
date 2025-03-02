using Newtonsoft.Json;

namespace DesafioMagalu.Utils
{
    public class DateJsonConverter : JsonConverter
    {
        private readonly string _dateFormat = "yyyy-MM-dd";

        public DateJsonConverter()
        {
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime date)
            {
                writer.WriteValue(date.ToString(_dateFormat));
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return DateTime.Parse(reader.Value.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
