using Faktur.Domain.Taxes;

namespace Faktur.Infrastructure.Converters;

internal class TaxCodeConverter : JsonConverter<TaxCodeUnit>
{
  public override TaxCodeUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return TaxCodeUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, TaxCodeUnit code, JsonSerializerOptions options)
  {
    writer.WriteStringValue(code.Value);
  }
}
