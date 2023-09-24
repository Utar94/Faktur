using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal record UpdateBannerCommand(string Id, UpdateBannerPayload Payload) : IRequest<AcceptedCommand>;
