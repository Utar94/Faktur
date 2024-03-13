using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Faktur.Application.Actors;

public record UserSignedInEvent(User User) : INotification;
