using Logitar.Faktur.Contracts.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Queries;

internal record ReadBannerQuery(string Id) : IRequest<Banner?>;
