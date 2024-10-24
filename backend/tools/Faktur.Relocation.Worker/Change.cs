using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;

namespace Faktur.Relocation.Worker;

internal record Change(DomainEvent Event, EventEntity Entity);
