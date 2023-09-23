using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Faktur.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly IConfiguration configuration;
  private readonly EventContext eventContext;
  private readonly FakturContext fakturContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, FakturContext fakturContext)
  {
    this.configuration = configuration;
    this.eventContext = eventContext;
    this.fakturContext = fakturContext;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    if (configuration.GetValue<bool>("EnableMigrations"))
    {
      await eventContext.Database.MigrateAsync(cancellationToken);
      await fakturContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
