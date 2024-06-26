﻿using Faktur.Contracts.Banners;
using Faktur.Domain.Banners;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Banners.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadBannerQueryTests : IntegrationTests
{
  private readonly IBannerRepository _bannerRepository;

  public ReadBannerQueryTests() : base()
  {
    _bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
  }

  [Fact(DisplayName = "It should return null when the banner cannot be found.")]
  public async Task It_should_return_null_when_the_banner_cannot_be_found()
  {
    ReadBannerQuery query = new(Guid.NewGuid());
    Assert.Null(await Mediator.Send(query));
  }

  [Fact(DisplayName = "It should return the banner when it is found.")]
  public async Task It_should_return_the_banner_when_it_is_found()
  {
    BannerAggregate banner = new(new DisplayNameUnit("MAXI"));
    await _bannerRepository.SaveAsync(banner);

    ReadBannerQuery query = new(banner.Id.ToGuid());
    Banner? result = await Mediator.Send(query);
    Assert.NotNull(result);
    Assert.Equal(banner.Id.ToGuid(), result.Id);
  }
}
