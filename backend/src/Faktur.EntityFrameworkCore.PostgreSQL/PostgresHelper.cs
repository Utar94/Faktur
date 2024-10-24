using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Faktur.EntityFrameworkCore.PostgreSQL;

internal class PostgresHelper : ISqlHelper
{
  public IInsertBuilder InsertInto(params ColumnId[] columns) => PostgresInsertBuilder.Into(columns);
  public IQueryBuilder QueryFrom(TableId table) => PostgresQueryBuilder.From(table);
}
