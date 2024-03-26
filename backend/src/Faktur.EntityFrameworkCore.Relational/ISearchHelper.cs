using Logitar.Data;
using Logitar.Portal.Contracts.Search;

namespace Faktur.EntityFrameworkCore.Relational;

public interface ISearchHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);
  ConditionalOperator CreateLikeOperator(string pattern);
}
