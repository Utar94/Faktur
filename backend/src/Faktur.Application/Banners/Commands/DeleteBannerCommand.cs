using Faktur.Application.Activities;
using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

public record DeleteBannerCommand(Guid Id) : Activity, IRequest<Banner?>;
