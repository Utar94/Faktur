using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository bannerRepository;

  public DeleteBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository bannerRepository)
  {
    this.applicationContext = applicationContext;
    this.bannerRepository = bannerRepository;
  }

  public async Task<AcceptedCommand> Handle(DeleteBannerCommand command, CancellationToken cancellationToken)
  {
    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate banner = await bannerRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    banner.Delete(applicationContext.ActorId);
    // TODO(fpion): remove banner from referencing stores

    await bannerRepository.SaveAsync(banner, cancellationToken);

    return applicationContext.AcceptCommand(banner);
  }
}
