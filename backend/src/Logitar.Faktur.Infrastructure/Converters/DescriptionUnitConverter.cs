using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Infrastructure.Converters;

public class DescriptionUnitConverter : JsonConverter<DescriptionUnit>
{
  public override DescriptionUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, DescriptionUnit description, JsonSerializerOptions options)
  {
    writer.WriteStringValue(description.Value);
  }
}
