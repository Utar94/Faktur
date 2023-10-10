namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal interface IMetadata
{
  long Version { get; }

  string CreatedBy { get; }
  DateTime CreatedOn { get; }

  string UpdatedBy { get; }
  DateTime UpdatedOn { get; }
}
