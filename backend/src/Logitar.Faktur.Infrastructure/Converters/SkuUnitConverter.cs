using Logitar.Faktur.Domain.Products;

namespace Logitar.Faktur.Infrastructure.Converters;

public class SkuUnitConverter : JsonConverter<SkuUnit>
{
  public override SkuUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SkuUnit sku, JsonSerializerOptions options)
  {
    writer.WriteStringValue(sku.Value);
  }
}
