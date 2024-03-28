using Faktur.Domain.Articles;
using Logitar.EventSourcing;

namespace Faktur.Application.Articles;

public class ArticleNotFoundException : AggregateNotFoundException<ArticleAggregate>
{
  protected override string ErrorMessage { get; } = "The specified article could not be found.";

  public ArticleNotFoundException(Guid articleId, string? propertyName = null) : base(new AggregateId(articleId), propertyName)
  {
  }
}
