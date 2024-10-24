using Logitar.Data;

namespace Faktur.EntityFrameworkCore.Relational;

public interface ISqlHelper
{
  IInsertBuilder InsertInto(params ColumnId[] columns);
  IQueryBuilder QueryFrom(TableId table);
}
