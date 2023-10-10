using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Stores;

[Trait(Traits.Category, Categories.Unit)]
public class StoreAggregateTests
{
  private readonly ActorId actorId;
  private readonly StoreAggregate store;

  public StoreAggregateTests()
  {
    actorId = ActorId.NewId();

    BannerAggregate banner = new(new DisplayNameUnit("MAXI"), actorId, BannerId.Parse("MAXI", nameof(BannerId)));

    StoreNumberUnit number = new("08772");
    store = new(new DisplayNameUnit("Maxi Drummondville"), actorId, new StoreId(banner, number))
    {
      Number = number
    };
    store.SetBanner(banner);

    DepartmentUnit department = new(new DepartmentNumberUnit("35"), new DisplayNameUnit("CHARCUTERIE"));
    store.SetDepartment(department, actorId);

    GtinUnit gtin = new("006740000010");
    ArticleAggregate article = new(new DisplayNameUnit("NOTRE-DAME BRIE"), actorId, new ArticleId(gtin))
    {
      Description = new DescriptionUnit("Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons.")
    };

    ProductUnit product = new(article.Id, article.DisplayName, department.Number, new SkuUnit("notre-dame-brie"),
      article.Description, new FlagsUnit("MRJ"), new UnitPriceUnit(6.99), new UnitTypeUnit("pc"));
    store.SetProduct(product, actorId);
  }

  [Fact(DisplayName = "RemoveDepartment: it should return false when the department could not be found.")]
  public void RemoveDepartment_it_should_return_false_when_the_department_could_not_be_found()
  {
    DepartmentNumberUnit number = new("33");
    Assert.False(store.RemoveDepartment(number, actorId));
  }

  [Fact(DisplayName = "RemoveDepartment: it should return true when the department could be found.")]
  public void RemoveDepartment_it_should_return_true_when_the_department_could_be_found()
  {
    DepartmentNumberUnit number = store.Departments.Keys.Single();
    Assert.True(store.RemoveDepartment(number, actorId));
    Assert.False(store.Departments.ContainsKey(number));
  }

  [Fact(DisplayName = "RemoveProduct: it should return false when the product could not be found.")]
  public void RemoveProduct_it_should_return_false_when_the_product_could_not_be_found()
  {
    ArticleId articleId = new(new GtinUnit("006740000018"));
    Assert.False(store.RemoveProduct(articleId, actorId));
  }

  [Fact(DisplayName = "RemoveProduct: it should return true when the product could be found.")]
  public void RemoveProduct_it_should_return_true_when_the_product_could_be_found()
  {
    ArticleId articleId = store.Products.Keys.Single();
    Assert.True(store.RemoveProduct(articleId, actorId));
    Assert.False(store.Products.ContainsKey(articleId));
  }

  [Fact(DisplayName = "SetBanner: it should remove the banner when it is null.")]
  public void SetBanner_it_should_remove_the_banner_when_it_is_null()
  {
    Assert.NotNull(store.BannerId);

    store.SetBanner(null);
    Assert.Null(store.BannerId);
  }

  [Fact(DisplayName = "SetBanner: it should update the banner when it is different.")]
  public void SetBanner_it_should_update_the_banner_when_it_is_different()
  {
    BannerAggregate banner = new(new DisplayNameUnit("IGA"), actorId, BannerId.Parse("IGA", nameof(BannerId)));
    Assert.NotEqual(banner.Id, store.BannerId);

    store.SetBanner(banner);
    Assert.Equal(banner.Id, store.BannerId);
  }

  [Fact(DisplayName = "SetDepartment: it should replace an existing department that is different.")]
  public void SetDepartment_it_should_replace_an_existing_department_that_is_different()
  {
    DepartmentUnit oldDepartment = store.Departments.Values.Single();

    DepartmentUnit department = new(oldDepartment.Number, oldDepartment.DisplayName, new DescriptionUnit("This is the department for fresh meat & cheese."));
    Assert.NotEqual(department, store.Departments[department.Number]);

    store.SetDepartment(department, actorId);
    Assert.Equal(department, store.Departments[department.Number]);
  }

  [Fact(DisplayName = "SetDepartment: it should set a new department.")]
  public void SetDepartment_it_should_set_a_new_department()
  {
    DepartmentUnit department = new(new DepartmentNumberUnit("33"), new DisplayNameUnit("BOULANGERIE"));

    store.SetDepartment(department);
    Assert.Equal(department, store.Departments[department.Number]);
  }

  [Fact(DisplayName = "SetProduct: it should replace an existing product that is different.")]
  public void SetProduct_it_should_replace_an_existing_product_that_is_different()
  {
    ProductUnit oldProduct = store.Products.Values.Single();

    ProductUnit product = new(oldProduct.ArticleId, oldProduct.DisplayName, oldProduct.DepartmentNumber, oldProduct.Sku,
      oldProduct.Description, oldProduct.Flags, new UnitPriceUnit(7.00), oldProduct.UnitType);
    Assert.NotNull(product.Sku);
    Assert.NotEqual(store.Products[product.ArticleId], product);
    Assert.NotEqual(store.ProductsBySku[product.Sku], product);

    store.SetProduct(product, actorId);
    Assert.Equal(store.Products[product.ArticleId], product);
    Assert.Equal(store.ProductsBySku[product.Sku], product);
  }

  [Fact(DisplayName = "SetProduct: it should set a new product.")]
  public void SetProduct_it_should_set_a_new_product()
  {
    GtinUnit gtin = new("006740000018");
    ArticleAggregate article = new(new DisplayNameUnit("AGRO CAMEMBERT"), actorId, new ArticleId(gtin));

    SkuUnit sku = new("agro-camembert");

    Assert.False(store.Products.ContainsKey(article.Id));
    Assert.False(store.ProductsBySku.ContainsKey(sku));

    ProductUnit product = new(article.Id, article.DisplayName, store.Departments.Keys.Single(), sku);
    store.SetProduct(product, actorId);

    Assert.True(store.Products.ContainsKey(article.Id));
    Assert.True(store.ProductsBySku.ContainsKey(sku));
  }

  [Fact(DisplayName = "SetProduct: it should throw SkuAlreadyUsedException when the SKU is already used.")]
  public void SetProduct_it_should_throw_SkuAlreadyUsedException_when_the_Sku_is_already_used()
  {
    GtinUnit gtin = new("006740000018");
    ArticleAggregate article = new(new DisplayNameUnit("AGRO CAMEMBERT"), actorId, new ArticleId(gtin));

    SkuUnit? sku = store.Products.Values.Single().Sku;
    ProductUnit product = new(article.Id, article.DisplayName, departmentNumber: null, sku);

    var exception = Assert.Throws<SkuAlreadyUsedException>(() => store.SetProduct(product, actorId));
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(sku, exception.Sku);
    Assert.Equal(nameof(ProductUnit.Sku), exception.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void ToString_it_should_return_the_correct_string_representation()
  {
    string expected = string.Format("{0} | {1} ({2})", store.DisplayName.Value, typeof(StoreAggregate), store.Id.Value);
    Assert.Equal(expected, store.ToString());
  }
}
