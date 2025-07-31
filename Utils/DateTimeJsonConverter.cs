using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace travel_agency_back.Utils
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private readonly string _format;
        // Construtor padr�o para uso com atributo [JsonConverter]
        public DateTimeJsonConverter() : this("dd/MM/yyyy") { }
        public DateTimeJsonConverter(string format)
        {
            _format = format;
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateTime.TryParseExact(value, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            throw new JsonException($"Data em formato inv�lido. Esperado: {_format}");
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
