using Faktur.Domain.Stores;

namespace Faktur.Infrastructure.Converters;

internal class StoreIdConverter : JsonConverter<StoreId>
{
  public override StoreId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return StoreId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, StoreId bannerId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(bannerId.Value);
  }
}
