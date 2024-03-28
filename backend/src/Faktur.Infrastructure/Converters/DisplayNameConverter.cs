using Logitar.Identity.Domain.Shared;

namespace Faktur.Infrastructure.Converters;

internal class DisplayNameConverter : JsonConverter<DisplayNameUnit>
{
  public override DisplayNameUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return DisplayNameUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, DisplayNameUnit displayName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(displayName.Value);
  }
}
