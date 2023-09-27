using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Articles;

namespace Logitar.Faktur.Infrastructure.Converters;

public class ArticleIdConverter : JsonConverter<ArticleId>
{
  public override ArticleId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? null : new(new AggregateId(value));
  }

  public override void Write(Utf8JsonWriter writer, ArticleId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
