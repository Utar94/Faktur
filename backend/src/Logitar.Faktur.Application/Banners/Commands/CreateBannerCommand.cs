using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal record CreateBannerCommand(CreateBannerPayload Payload) : IRequest<AcceptedCommand>;
