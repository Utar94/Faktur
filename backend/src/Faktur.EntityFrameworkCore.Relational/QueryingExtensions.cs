using Logitar.Data;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder builder, ColumnId column, IEnumerable<Guid> ids)
  {
    if (!ids.Any())
    {
      return builder;
    }

    string[] aggregateIds = ids.Distinct().Select(id => new AggregateId(id).Value).ToArray();

    return builder.Where(column, Operators.IsIn(aggregateIds));
  }
}
