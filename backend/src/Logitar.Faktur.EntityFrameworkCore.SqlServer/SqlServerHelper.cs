using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Faktur.EntityFrameworkCore.Relational;

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer;

internal class SqlServerHelper : SqlHelper, ISqlHelper
{
  public IQueryBuilder QueryFrom(TableId table) => SqlServerQueryBuilder.From(table);
}
