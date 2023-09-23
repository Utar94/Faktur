using Logitar.Data;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.EntityFrameworkCore.Relational;

public abstract class SqlHelper
{
  public IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns)
  {
    int termCount = search.Terms.Count;
    if (termCount == 0 || !columns.Any())
    {
      return builder;
    }

    List<Condition> conditions = new(capacity: termCount);
    foreach (SearchTerm term in search.Terms)
    {
      if (!string.IsNullOrWhiteSpace(term.Value))
      {
        string pattern = term.Value.Trim();
        conditions.Add(columns.Length == 1
          ? new OperatorCondition(columns.Single(), CreateOperator(pattern))
          : new OrCondition(columns.Select(column => new OperatorCondition(column, CreateOperator(pattern))).ToArray()));
      }
    }

    if (conditions.Any())
    {
      switch (search.Operator)
      {
        case SearchOperator.And:
          return builder.WhereAnd(conditions.ToArray());
        case SearchOperator.Or:
          return builder.WhereOr(conditions.ToArray());
      }
    }

    return builder;
  }
  protected virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);
}
