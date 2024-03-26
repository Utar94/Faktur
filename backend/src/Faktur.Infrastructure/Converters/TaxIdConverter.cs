using Faktur.Domain.Taxes;

namespace Faktur.Infrastructure.Converters;

internal class TaxIdConverter : JsonConverter<TaxId>
{
  public override TaxId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return TaxId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, TaxId taxId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(taxId.Value);
  }
}
