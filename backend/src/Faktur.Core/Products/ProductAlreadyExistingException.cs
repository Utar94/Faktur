using Logitar.WebApiToolKit.Core.Exceptions;
using System.Text;

namespace Faktur.Core.Products
{
  public class ProductAlreadyExistingException : ConflictException
  {
    public ProductAlreadyExistingException(
      int storeId,
      int articleId,
      string? paramName = null,
      string? message = null,
      Exception? innerException = null
    ) : base(paramName, message ?? GetMessage(storeId, articleId), innerException)
    {
      ArticleId = articleId;
      StoreId = storeId;
    }

    public int ArticleId { get; }
    public int StoreId { get; }

    private static string GetMessage(int storeId, int articleId)
    {
      var message = new StringBuilder();

      message.AppendLine("The product already exists.");
      message.AppendLine($"StoreId: {storeId}");
      message.AppendLine($"ArticleId: {articleId}");

      return message.ToString();
    }
  }
}
