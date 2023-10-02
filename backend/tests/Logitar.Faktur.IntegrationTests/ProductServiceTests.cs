using Logitar.EventSourcing;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

[Trait(Traits.Category, Categories.Integration)]
public class ProductServiceTests : IntegrationTests
{
  private readonly IArticleRepository articleRepository;
  private readonly IBannerRepository bannerRepository;
  private readonly IProductService productService;
  private readonly IStoreRepository storeRepository;

  private readonly ArticleAggregate article;
  private readonly BannerAggregate banner;
  private readonly StoreAggregate store;

  public ProductServiceTests()
  {
    articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    bannerRepository = ServiceProvider.GetRequiredService<IBannerRepository>();
    productService = ServiceProvider.GetRequiredService<IProductService>();
    storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    BannerId bannerId = new(new AggregateId("MAXI"));
    banner = new(new DisplayNameUnit("Maxi"), ApplicationContext.ActorId, bannerId);

    StoreNumberUnit number = new("08772");
    StoreId storeId = new(banner, number);
    store = new StoreAggregate(new DisplayNameUnit("Maxi Drummondville"), ApplicationContext.ActorId, storeId)
    {
      Address = new AddressUnit("1870 boul Saint-Joseph", "Drummondville", "CA", "QC", "J2B 1R2"),
      Phone = new PhoneUnit("+18194721197", "CA")
    };
    store.Update(ApplicationContext.ActorId);

    GtinUnit gtin = new("006740000010");
    article = new(new DisplayNameUnit("NOTRE-DAME BRIE"), ApplicationContext.ActorId, new ArticleId(gtin))
    {
      Gtin = gtin
    };
    article.Update(ApplicationContext.ActorId);

    DepartmentUnit department = new(new DepartmentNumberUnit("35"), new DisplayNameUnit("CHARCUTERIE"));
    store.SetDepartment(department, ApplicationContext.ActorId);

    SkuUnit sku = new("notre-dame-brie");
    ProductUnit product = new(article.Id, article.DisplayName, departmentNumber: null, sku);
    store.SetProduct(product, ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await articleRepository.SaveAsync(article);
    await bannerRepository.SaveAsync(banner);
    await storeRepository.SaveAsync(store);
  }

  [Fact(DisplayName = "SaveAsync: it should save a new product.")]
  public async Task SaveAsync_it_should_save_a_new_product()
  {
    GtinUnit gtin = new("006740000018");
    ArticleAggregate otherArticle = new(new DisplayNameUnit("AGRO CAMEMBERT"), ApplicationContext.ActorId, new ArticleId(gtin))
    {
      Gtin = gtin,
      Description = new DescriptionUnit("Issu de la plus pure tradition européenne, un véritable camembert qui offre un goût crémeux de lait, de noisettes et de champignons. Sa saveur et sa texture en ont fait un des favoris au Canada.")
    };
    await articleRepository.SaveAsync(otherArticle);

    SaveProductPayload payload = new()
    {
      DepartmentNumber = "    ",
      Sku = "    ",
      DisplayName = $"  {otherArticle.DisplayName.Value}  ",
      Description = "    ",
      Flags = "    ",
      UnitType = "    "
    };

    AcceptedCommand command = await productService.SaveAsync(store.Id.Value, otherArticle.Id.Value, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ProductEntity? product = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Article!.AggregateId == otherArticle.Id.Value);
    Assert.NotNull(product);
    Assert.NotNull(product.Store);
    Assert.Equal(store.Id.Value, product.Store.AggregateId);
    Assert.Null(product.Department);
    Assert.NotNull(product.Article);
    Assert.Equal(otherArticle.Id.Value, product.Article.AggregateId);

    Assert.Null(product.Sku);
    Assert.Equal(payload.DisplayName.Trim(), product.DisplayName);
    Assert.Null(product.Description);
    Assert.Null(product.Flags);
    Assert.Null(product.UnitPrice);
    Assert.Null(product.UnitType);

    Assert.Equal(1, product.Version);
    Assert.Equal(ApplicationContext.Actor.Id, product.CreatedBy);
    AssertIsNear(AsUniversalTime(product.CreatedOn));
    Assert.Equal(ApplicationContext.Actor.Id, product.UpdatedBy);
    Assert.Equal(product.CreatedOn, product.UpdatedOn);
  }

  [Fact(DisplayName = "SaveAsync: it should save an existing product.")]
  public async Task SaveAsync_it_should_save_an_existing_product()
  {
    Assert.NotNull(article.Gtin);
    SaveProductPayload payload = new()
    {
      DepartmentNumber = store.Departments.Values.Single().Number.Value,
      Sku = article.Gtin.Value,
      DisplayName = $" {article.DisplayName.Value} ",
      Description = " Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons. ",
      Flags = " MRJ ",
      UnitPrice = 6.00,
      UnitType = " pc "
    };

    AcceptedCommand command = await productService.SaveAsync(store.Id.Value, article.Id.Value, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ProductEntity? product = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Article!.AggregateId == article.Id.Value);
    Assert.NotNull(product);
    Assert.NotNull(product.Store);
    Assert.Equal(store.Id.Value, product.Store.AggregateId);

    Assert.Equal(int.Parse(payload.DepartmentNumber), product.Department?.NumberNormalized);
    Assert.Equal(payload.Sku.Trim(), product.Sku);
    Assert.Equal(payload.DisplayName.Trim(), product.DisplayName);
    Assert.Equal(payload.Description.Trim(), product.Description);
    Assert.Equal(payload.Flags.Trim(), product.Flags);
    Assert.Equal(payload.UnitPrice, product.UnitPrice);
    Assert.Equal(payload.UnitType.Trim(), product.UnitType);

    Assert.Equal(2, product.Version);
    Assert.Equal(ApplicationContext.Actor.Id, product.CreatedBy);
    Assert.Equal(ApplicationContext.Actor.Id, product.UpdatedBy);
    AssertIsNear(AsUniversalTime(product.UpdatedOn));
    Assert.True(product.UpdatedOn > product.CreatedOn);
  }

  [Fact(DisplayName = "SaveAsync: it should throw AggregateNotFoundException when the article could not be found.")]
  public async Task SaveAsync_it_should_throw_AggregateNotFoundException_when_the_article_could_not_be_found()
  {
    SaveProductPayload payload = new()
    {
      DisplayName = "NOTRE-DAME BRIE"
    };

    AggregateId articleId = new("006740000018");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ArticleAggregate>>(
      async () => await productService.SaveAsync(store.Id.Value, articleId.Value, payload)
    );
    Assert.Equal(articleId, exception.Id);
    Assert.Equal("ArticleId", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task SaveAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    SaveProductPayload payload = new()
    {
      DisplayName = "NOTRE-DAME BRIE"
    };

    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await productService.SaveAsync(storeId.Value, article.Id.Value, payload)
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw DepartmentNotFoundException when the department could not be found.")]
  public async Task SaveAsync_it_should_throw_DepartmentNotFoundException_when_the_department_could_not_be_found()
  {
    SaveProductPayload payload = new()
    {
      DepartmentNumber = "1",
      DisplayName = "NOTRE-DAME BRIE"
    };

    var exception = await Assert.ThrowsAsync<DepartmentNotFoundException>(
      async () => await productService.SaveAsync(store.Id.Value, article.Id.Value, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(payload.DepartmentNumber, exception.Number.Value);
    Assert.Equal(nameof(payload.DepartmentNumber), exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw SkuAlreadyUsedException when the SKU is already used.")]
  public async Task SaveAsync_it_should_throw_SkuAlreadyUsedException_when_the_Sku_is_already_used()
  {
    GtinUnit gtin = new("006740000018");
    ArticleAggregate otherArticle = new(new DisplayNameUnit("AGRO CAMEMBERT"), ApplicationContext.ActorId, new ArticleId(gtin))
    {
      Gtin = gtin,
      Description = new DescriptionUnit("Issu de la plus pure tradition européenne, un véritable camembert qui offre un goût crémeux de lait, de noisettes et de champignons. Sa saveur et sa texture en ont fait un des favoris au Canada.")
    };
    await articleRepository.SaveAsync(otherArticle);

    Assert.NotNull(article.Gtin);
    SaveProductPayload payload = new()
    {
      Sku = "notre-dame-brie",
      DisplayName = otherArticle.DisplayName.Value
    };

    var exception = await Assert.ThrowsAsync<SkuAlreadyUsedException>(
      async () => await productService.SaveAsync(store.Id.Value, otherArticle.Id.Value, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(payload.Sku, exception.Sku.Value);
    Assert.Equal(nameof(ProductUnit.Sku), exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task SaveAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveProductPayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(
      async () => await productService.SaveAsync(store.Id.Value, article.Id.Value, payload)
    );
  }
}
