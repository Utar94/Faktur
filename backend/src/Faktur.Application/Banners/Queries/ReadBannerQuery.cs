using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.Application.Banners.Queries;

public record ReadBannerQuery(Guid Id) : IRequest<Banner?>;
