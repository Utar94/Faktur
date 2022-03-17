using Logitar.WebApiToolKit.Core.Exceptions;
using System.Text;

namespace Faktur.Core.Products
{
  public class SkuAlreadyTakenException : ConflictException
  {
    public SkuAlreadyTakenException(
      int storeId,
      string sku,
      string? paramName = null,
      string? message = null,
      Exception? innerException = null
    ) : base(paramName, message ?? GetMessage(storeId, sku), innerException)
    {
      Sku = sku ?? throw new ArgumentNullException(nameof(sku));
      StoreId = storeId;
    }

    public string Sku { get; }
    public int StoreId { get; }

    private static string GetMessage(int storeId, string sku)
    {
      var message = new StringBuilder();

      message.AppendLine("The product already exists.");
      message.AppendLine($"StoreId: {storeId}");
      message.AppendLine($"Sku: {sku}");

      return message.ToString();
    }
  }
}
