namespace Faktur.Core.Models
{
  public class ListModel<T>
  {
    public ListModel(IEnumerable<T> items, long? total = null)
    {
      Items = items ?? throw new ArgumentNullException(nameof(items));
      Total = total ?? items.LongCount();
    }

    public IEnumerable<T> Items { get; }
    public long Total { get; }
  }
}
