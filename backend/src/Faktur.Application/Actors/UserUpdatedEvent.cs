using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Faktur.Application.Actors;

public record UserUpdatedEvent(User User) : INotification;
