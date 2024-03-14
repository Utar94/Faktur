using Logitar;

namespace Faktur.Application.Articles;

public class ArticleNotFoundException : Exception
{
  private const string ErrorMessage = "The specified article could not be found.";

  public Guid ArticleId
  {
    get => (Guid)Data[nameof(ArticleId)]!;
    private set => Data[nameof(ArticleId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ArticleNotFoundException(Guid articleId, string? propertyName = null) : base(BuildMessage(articleId, propertyName))
  {
    ArticleId = articleId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid articleId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ArticleId), articleId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
