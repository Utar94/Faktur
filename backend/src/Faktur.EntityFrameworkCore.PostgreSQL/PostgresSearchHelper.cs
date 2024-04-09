using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Faktur.EntityFrameworkCore.PostgreSQL;

internal class PostgresSearchHelper : SearchHelper
{
  public override ConditionalOperator CreateLikeOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
