using Faktur.Application.Activities;
using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

public record CreateBannerCommand(CreateBannerPayload Payload) : Activity, IRequest<Banner>;
