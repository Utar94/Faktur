using Logitar.Faktur.Domain.Departments;

namespace Logitar.Faktur.Infrastructure.Converters;

public class DepartmentNumberUnitConverter : JsonConverter<DepartmentNumberUnit>
{
  public override DepartmentNumberUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, DepartmentNumberUnit number, JsonSerializerOptions options)
  {
    writer.WriteStringValue(number.Value);
  }
}
