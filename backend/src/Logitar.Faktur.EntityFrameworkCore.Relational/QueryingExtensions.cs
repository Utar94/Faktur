using Logitar.Data;
using Logitar.Faktur.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdIn(this IQueryBuilder builder, IEnumerable<string> ids, ColumnId column)
  {
    if (!ids.Any())
    {
      return builder;
    }

    string[] aggregateIds = ids.Select(id => Guid.TryParse(id, out Guid guid) ? guid.ToString() : id)
      .Distinct()
      .ToArray();

    return builder.Where(column, Operators.IsIn(aggregateIds));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    if (payload.Skip > 0)
    {
      query = query.Skip(payload.Skip);
    }
    if (payload.Limit > 0)
    {
      query = query.Take(payload.Limit);
    }

    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> set, IQueryBuilder builder) where T : class
    => set.FromQuery(builder.Build());
  public static IQueryable<T> FromQuery<T>(this DbSet<T> set, IQuery query) where T : class
    => set.FromSqlRaw(query.Text, query.Parameters.ToArray());
}
