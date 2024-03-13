using Faktur.Domain.Articles;

namespace Faktur.Infrastructure.Converters;

internal class GtinConverter : JsonConverter<GtinUnit>
{
  public override GtinUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return GtinUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, GtinUnit gtin, JsonSerializerOptions options)
  {
    writer.WriteStringValue(gtin.Value);
  }
}
