using Faktur.Domain.Stores;

namespace Faktur.Infrastructure.Converters;

internal class NumberConverter : JsonConverter<NumberUnit>
{
  public override NumberUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return NumberUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, NumberUnit number, JsonSerializerOptions options)
  {
    writer.WriteStringValue(number.Value);
  }
}
