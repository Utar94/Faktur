﻿using Faktur.Core.Products;
using Faktur.Core.Receipts;

namespace Faktur.Core.Stores
{
  public class Store : Aggregate
  {
    public Store(Guid userId) : base(userId)
    {
    }
    private Store() : base()
    {
    }

    public Banner? Banner { get; set; }
    public int? BannerId { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public string? Number { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }

    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
