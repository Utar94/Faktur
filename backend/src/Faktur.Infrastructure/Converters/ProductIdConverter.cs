using Faktur.Domain.Products;

namespace Faktur.Infrastructure.Converters;

internal class ProductIdConverter : JsonConverter<ProductId>
{
  public override ProductId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return ProductId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, ProductId bannerId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(bannerId.Value);
  }
}
