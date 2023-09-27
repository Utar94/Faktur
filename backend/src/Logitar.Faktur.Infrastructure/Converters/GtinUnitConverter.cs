using Logitar.Faktur.Domain.Articles;

namespace Logitar.Faktur.Infrastructure.Converters;

public class GtinUnitConverter : JsonConverter<GtinUnit>
{
  public override GtinUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, GtinUnit gtin, JsonSerializerOptions options)
  {
    writer.WriteStringValue(gtin.Value);
  }
}
