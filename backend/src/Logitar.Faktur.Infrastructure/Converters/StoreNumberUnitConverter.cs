using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Infrastructure.Converters;

public class StoreNumberUnitConverter : JsonConverter<StoreNumberUnit>
{
  public override StoreNumberUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, StoreNumberUnit number, JsonSerializerOptions options)
  {
    writer.WriteStringValue(number.Value);
  }
}
