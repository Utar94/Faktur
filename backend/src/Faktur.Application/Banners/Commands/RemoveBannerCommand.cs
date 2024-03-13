using Faktur.Domain.Banners;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Banners.Commands;

internal record RemoveBannerCommand(ActorId ActorId, BannerAggregate Banner) : INotification;
