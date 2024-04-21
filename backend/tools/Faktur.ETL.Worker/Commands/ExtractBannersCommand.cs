using Faktur.Contracts.Banners;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal record ExtractBannersCommand(Mapper Mapper) : IRequest<IEnumerable<Banner>>;
