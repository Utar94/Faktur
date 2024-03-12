using Faktur.Domain.Shared;

namespace Faktur.Infrastructure.Converters;

internal class DescriptionConverter : JsonConverter<DescriptionUnit>
{
  public override DescriptionUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return DescriptionUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, DescriptionUnit description, JsonSerializerOptions options)
  {
    writer.WriteStringValue(description.Value);
  }
}
