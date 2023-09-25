namespace Logitar.Faktur.Contracts.Stores;

public interface IPhone
{
  string? CountryCode { get; }
  string Number { get; }
  string? Extension { get; }
}
