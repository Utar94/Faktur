using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Infrastructure.Converters;

public class DisplayNameUnitConverter : JsonConverter<DisplayNameUnit>
{
  public override DisplayNameUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, DisplayNameUnit displayName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(displayName.Value);
  }
}
