using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;

namespace Logitar.Faktur.Application.Extensions;

internal static class ApplicationContextExtensions
{
  public static AcceptedCommand AcceptCommand(this IApplicationContext context, AggregateRoot aggregate) => new()
  {
    AggregateId = aggregate.Id.Value,
    AggregateVersion = aggregate.Version,
    Actor = context.Actor,
    Timestamp = aggregate.UpdatedOn
  };
}
