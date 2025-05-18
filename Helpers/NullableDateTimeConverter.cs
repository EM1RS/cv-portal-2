using System.Text.Json;
using System.Text.Json.Serialization;

public class NullableDateTimeConverter : JsonConverter<DateTime?> // Her Converterer vi NullableDateTime
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue))
                return null;

            if (DateTime.TryParse(stringValue, out var result))
                return result;

            throw new JsonException($"Ugyldig datoformat: {stringValue}");
        }

        if (reader.TokenType == JsonTokenType.Null)
            return null;

        throw new JsonException("Ugyldig JSON for DateTime?");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("O")); 
        else
            writer.WriteNullValue();
    }
}
