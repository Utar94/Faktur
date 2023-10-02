using Logitar.Faktur.Domain.Products;

namespace Logitar.Faktur.Infrastructure.Converters;

public class UnitPriceUnitConverter : JsonConverter<UnitPriceUnit>
{
  public override UnitPriceUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    double? value = reader.GetDouble();

    return value.HasValue ? new(value.Value) : null;
  }

  public override void Write(Utf8JsonWriter writer, UnitPriceUnit unitPrice, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(unitPrice.Value);
  }
}
