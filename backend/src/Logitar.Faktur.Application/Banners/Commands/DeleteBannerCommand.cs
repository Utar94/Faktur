using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal record DeleteBannerCommand(string Id) : IRequest<AcceptedCommand>;
