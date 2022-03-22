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
    public ICollection<Store> Stores { get; set; } = new List<Store>();

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
