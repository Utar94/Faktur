using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.ETL.Worker.Commands;

internal class ImportBannersCommandHandler : IRequestHandler<ImportBannersCommand, int>
{
  private readonly IBannerRepository _bannerRepository;

  public ImportBannersCommandHandler(IBannerRepository bannerRepository)
  {
    _bannerRepository = bannerRepository;
  }

  public async Task<int> Handle(ImportBannersCommand command, CancellationToken cancellationToken)
  {
    Dictionary<Guid, BannerAggregate> banners = (await _bannerRepository.LoadAsync(cancellationToken))
      .ToDictionary(x => x.Id.ToGuid(), x => x);
    int count = 0;
    foreach (Banner banner in command.Banners)
    {
      BannerId id = new(banner.Id);

      DisplayNameUnit displayName = new(banner.DisplayName);
      if (banners.TryGetValue(banner.Id, out BannerAggregate? existingBanner))
      {
        existingBanner.DisplayName = displayName;
      }
      else
      {
        ActorId createdBy = new(banner.CreatedBy.Id);
        existingBanner = new(displayName, createdBy, id);
        banners[banner.Id] = existingBanner;
      }

      existingBanner.Description = DescriptionUnit.TryCreate(banner.Description);

      ActorId updatedBy = new(banner.UpdatedBy.Id);
      existingBanner.Update(updatedBy);

      if (existingBanner.HasChanges)
      {
        count++;
      }
    }

    await _bannerRepository.SaveAsync(banners.Values, cancellationToken);

    return count;
  }
}
