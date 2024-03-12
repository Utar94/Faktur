using Logitar.Data;
using Logitar.Portal.Contracts.Search;

namespace Faktur.EntityFrameworkCore.Relational;

public abstract class SearchHelper : ISearchHelper
{
  public IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns)
  {
    int termCount = search.Terms.Count;
    if (termCount == 0 || columns.Length == 0)
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

    if (conditions.Count > 0)
    {
      switch (search.Operator)
      {
        case SearchOperator.And:
          return builder.WhereAnd([.. conditions]);
        case SearchOperator.Or:
          return builder.WhereOr([.. conditions]);
      }
    }

    return builder;
  }
  protected virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);
}
