using Logitar.Data;

namespace Faktur.EntityFrameworkCore.Relational;

public interface ISqlHelper
{
  IQueryBuilder QueryFrom(TableId table);
}
