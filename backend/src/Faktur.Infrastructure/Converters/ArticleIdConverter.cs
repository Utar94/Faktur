using Faktur.Domain.Articles;

namespace Faktur.Infrastructure.Converters;

internal class ArticleIdConverter : JsonConverter<ArticleId>
{
  public override ArticleId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return ArticleId.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, ArticleId articleId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(articleId.Value);
  }
}
