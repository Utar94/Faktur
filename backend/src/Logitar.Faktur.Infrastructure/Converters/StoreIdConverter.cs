using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Infrastructure.Converters;

public class StoreIdConverter : JsonConverter<StoreId>
{
  public override StoreId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(new AggregateId(value));
  }

  public override void Write(Utf8JsonWriter writer, StoreId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
