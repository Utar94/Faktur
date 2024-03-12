using Faktur.Application.Activities;
using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

public record UpdateBannerCommand(Guid Id, UpdateBannerPayload Payload) : Activity, IRequest<Banner?>;
