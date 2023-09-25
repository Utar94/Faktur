namespace Logitar.Faktur.Contracts.Stores;

public interface IAddress
{
  string Street { get; }
  string Locality { get; }
  string? Region { get; }
  string? PostalCode { get; }
  string Country { get; }
}
