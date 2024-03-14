using Faktur.Domain.Products;

namespace Faktur.Infrastructure.Converters;

internal class FlagsConverter : JsonConverter<FlagsUnit>
{
  public override FlagsUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return FlagsUnit.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, FlagsUnit flags, JsonSerializerOptions options)
  {
    writer.WriteStringValue(flags.Value);
  }
}
