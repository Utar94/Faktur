namespace Faktur.Core.Stores
{
  public class Banner : Aggregate
  {
    public Banner(Guid userId) : base(userId)
    {
    }
    private Banner() : base()
    {
    }

    public string? Description { get; set; }
    public string Name { get; set; } = null!;
  }
}
