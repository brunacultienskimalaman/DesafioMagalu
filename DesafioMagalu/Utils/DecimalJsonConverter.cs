using Newtonsoft.Json;
using System.Globalization;

namespace DesafioMagalu.Utils
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        private readonly string _format;

        public DecimalJsonConverter()
        {
        }
        public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("0.00", CultureInfo.InvariantCulture));
        }

        public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }

}
