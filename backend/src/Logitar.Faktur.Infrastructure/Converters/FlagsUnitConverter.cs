using Logitar.Faktur.Domain.Products;

namespace Logitar.Faktur.Infrastructure.Converters;

public class FlagsUnitConverter : JsonConverter<FlagsUnit>
{
  public override FlagsUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, FlagsUnit flags, JsonSerializerOptions options)
  {
    writer.WriteStringValue(flags.Value);
  }
}
