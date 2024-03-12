using Faktur.Application.Activities;
using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Commands;

public record ReplaceBannerCommand(Guid Id, ReplaceBannerPayload Payload, long? Version) : Activity, IRequest<Banner?>;
