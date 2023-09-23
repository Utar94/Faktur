using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using MediatR;

namespace Logitar.Faktur.Infrastructure;

internal class EventBus : IEventBus
{
  private readonly IPublisher publisher;

  public EventBus(IPublisher publisher)
  {
    this.publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken)
  {
    await publisher.Publish(change, cancellationToken);
  }
}
