using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Faktur.EntityFrameworkCore.SqlServer;

internal class SqlServerHelper : ISqlHelper
{
  public IQueryBuilder QueryFrom(TableId table) => SqlServerQueryBuilder.From(table);
}
