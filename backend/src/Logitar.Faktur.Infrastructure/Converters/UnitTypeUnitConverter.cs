using Logitar.Faktur.Domain.Products;

namespace Logitar.Faktur.Infrastructure.Converters;

public class UnitTypeUnitConverter : JsonConverter<UnitTypeUnit>
{
  public override UnitTypeUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, UnitTypeUnit unitType, JsonSerializerOptions options)
  {
    writer.WriteStringValue(unitType.Value);
  }
}
