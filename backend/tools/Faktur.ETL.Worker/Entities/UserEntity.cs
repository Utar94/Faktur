namespace Faktur.ETL.Worker.Entities;

internal class UserEntity
{
  public Guid Id { get; set; }

  public string? UserName { get; set; }

  public string? Email { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  public string? Picture { get; set; }
}
