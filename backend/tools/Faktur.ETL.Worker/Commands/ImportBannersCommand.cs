using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ImportBannersCommand(IEnumerable<Banner> Banners) : IRequest<int>;
