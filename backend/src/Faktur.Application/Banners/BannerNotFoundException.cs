using Logitar;

namespace Faktur.Application.Banners;

public class BannerNotFoundException : Exception
{
  private const string ErrorMessage = "The specified banner could not be found.";

  public Guid BannerId
  {
    get => (Guid)Data[nameof(BannerId)]!;
    private set => Data[nameof(BannerId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public BannerNotFoundException(Guid bannerId, string? propertyName = null) : base(BuildMessage(bannerId, propertyName))
  {
    BannerId = bannerId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid bannerId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(BannerId), bannerId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
