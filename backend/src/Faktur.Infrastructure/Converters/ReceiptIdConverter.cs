using Faktur.Domain.Receipts;

namespace Faktur.Infrastructure.Converters;

internal class ReceiptIdConverter : JsonConverter<ReceiptId>
{
  public override ReceiptId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return ReceiptId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, ReceiptId receiptId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(receiptId.Value);
  }
}
