using Faktur.Domain.Products;

namespace Faktur.Infrastructure.Converters;

internal class SkuConverter : JsonConverter<SkuUnit>
{
  public override SkuUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return SkuUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, SkuUnit sku, JsonSerializerOptions options)
  {
    writer.WriteStringValue(sku.Value);
  }
}
