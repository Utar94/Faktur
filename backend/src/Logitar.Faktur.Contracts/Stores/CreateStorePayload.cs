﻿namespace Logitar.Faktur.Contracts.Stores;

public record CreateStorePayload
{
  public string? Id { get; set; }

  public string? Number { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public AddressPayload? Address { get; set; }
  public PhonePayload? Phone { get; set; }
}
