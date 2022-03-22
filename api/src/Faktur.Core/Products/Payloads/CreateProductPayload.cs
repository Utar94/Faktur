namespace Faktur.Core.Products.Payloads
{
  public class CreateProductPayload : SaveProductPayload
  {
    public int ArticleId { get; set; }
    public int StoreId { get; set; }
  }
}
