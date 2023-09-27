using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Banners;

namespace Logitar.Faktur.Infrastructure.Converters;

public class BannerIdConverter : JsonConverter<BannerId>
{
  public override BannerId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(new AggregateId(value));
  }

  public override void Write(Utf8JsonWriter writer, BannerId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
