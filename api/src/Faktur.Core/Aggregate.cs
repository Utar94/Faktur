namespace Faktur.Core
{
  public abstract class Aggregate
  {
    protected Aggregate()
    {
    }
    protected Aggregate(Guid userId)
    {
      CreatedAt = DateTime.UtcNow;
      CreatedById = userId;
      Key = Guid.NewGuid();
    }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public bool Deleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }
    public int Id { get; set; }
    public Guid Key { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public int Version { get; set; }

    public void Delete(Guid userId)
    {
      Deleted = true;
      DeletedAt = DateTime.UtcNow;
      DeletedById = userId;
    }

    public void Update(Guid userId)
    {
      UpdatedAt = DateTime.UtcNow;
      UpdatedById = userId;
      Version++;
    }

    public override bool Equals(object? obj) => obj is Aggregate aggregate
      && aggregate.GetType().Equals(GetType())
      && aggregate.Id == Id;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public override string ToString() => $"{GetType()} (Id={Id})";
  }
}
