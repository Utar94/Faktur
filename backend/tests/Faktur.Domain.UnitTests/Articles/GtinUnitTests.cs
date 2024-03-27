using Faktur.Domain.Shared;

namespace Faktur.Domain.Articles;

[Trait(Traits.Category, Categories.Unit)]
public class GtinUnitTests
{
  [Theory(DisplayName = "ctor: it should create the correct GTIN.")]
  [InlineData("06038385904")]
  public void ctor_it_should_create_the_correct_Gtin(string value)
  {
    GtinUnit gtin = new($"  {value}  ");
    Assert.Equal(value.Trim(), gtin.Value);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the value is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_value_is_not_valid()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new GtinUnit("test"));
    Assert.Equal(nameof(AllowedCharactersValidator), Assert.Single(exception.Errors).ErrorCode);
  }

  [Theory(DisplayName = "NormalizedValue: it should return the correct normalized value.")]
  [InlineData("06038385904", 6038385904)]
  public void NormalizedValue_it_should_return_the_correct_normalized_value(string value, long normalized)
  {
    GtinUnit gtin = new(value);
    Assert.Equal(normalized, gtin.NormalizedValue);
  }

  [Theory(DisplayName = "TryCreate: it should return null when the value is null or white space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("    ")]
  public void TryCreate_it_should_return_null_when_the_value_is_null_or_white_space(string? value)
  {
    Assert.Null(GtinUnit.TryCreate(value));
  }

  [Theory(DisplayName = "TryCreate: it should return the correct created GTIN.")]
  [InlineData("  06038385904  ")]

  public void TryCreate_it_should_return_the_correct_created_Gtin(string value)
  {
    GtinUnit? gtin = GtinUnit.TryCreate(value);
    Assert.NotNull(gtin);
    Assert.Equal(value.Trim(), gtin.Value);
  }
}
