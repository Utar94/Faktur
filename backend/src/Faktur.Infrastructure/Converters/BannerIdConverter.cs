using Faktur.Domain.Banners;

namespace Faktur.Infrastructure.Converters;

internal class BannerIdConverter : JsonConverter<BannerId>
{
  public override BannerId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return BannerId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, BannerId bannerId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(bannerId.Value);
  }
}
