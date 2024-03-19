using Faktur.Contracts.Banners;
using Faktur.EntityFrameworkCore.Relational;
using FluentValidation.Results;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Application.Banners.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateBannerCommandTests : IntegrationTests
{
  public CreateBannerCommandTests() : base()
  {
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Banners.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should create a new banner.")]
  public async Task It_should_create_a_new_banner()
  {
    CreateBannerPayload payload = new("MAXI");
    CreateBannerCommand command = new(payload);
    Banner banner = await Mediator.Send(command);

    Assert.Equal(1, banner.Version);
    Assert.Equal(banner.CreatedOn, banner.UpdatedOn);
    Assert.Equal(Actor, banner.CreatedBy);
    Assert.Equal(Actor, banner.UpdatedBy);

    Assert.Equal(payload.DisplayName, banner.DisplayName);
    Assert.Equal(payload.Description, banner.Description);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateBannerPayload payload = new(displayName: string.Empty);
    CreateBannerCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
