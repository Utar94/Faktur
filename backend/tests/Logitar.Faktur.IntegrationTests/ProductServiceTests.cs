using Logitar.EventSourcing;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Products;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
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

  private readonly ArticleAggregate brie;
  private readonly ArticleAggregate camembert;
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

    GtinUnit brieGtin = new("006740000010");
    brie = new(new DisplayNameUnit("NOTRE-DAME BRIE"), ApplicationContext.ActorId, new ArticleId(brieGtin))
    {
      Gtin = brieGtin
    };
    brie.Update(ApplicationContext.ActorId);

    GtinUnit camembertGtin = new("006740000018");
    camembert = new(new DisplayNameUnit("AGRO CAMEMBERT"), ApplicationContext.ActorId, new ArticleId(camembertGtin))
    {
      Gtin = camembertGtin,
      Description = new DescriptionUnit("Issu de la plus pure tradition européenne, un véritable camembert qui offre un goût crémeux de lait, de noisettes et de champignons. Sa saveur et sa texture en ont fait un des favoris au Canada.")
    };

    DepartmentUnit department = new(new DepartmentNumberUnit("35"), new DisplayNameUnit("CHARCUTERIE"));
    store.SetDepartment(department, ApplicationContext.ActorId);

    SkuUnit sku = new("notre-dame-brie");
    ProductUnit product = new(brie.Id, brie.DisplayName, department.Number, sku);
    store.SetProduct(product, ApplicationContext.ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await articleRepository.SaveAsync(new[] { brie, camembert });
    await bannerRepository.SaveAsync(banner);
    await storeRepository.SaveAsync(store);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct product by ID.")]
  public async Task ReadAsync_it_should_read_the_correct_product_by_id()
  {
    ProductUnit product = store.Products.Values.Single();

    Product? result = await productService.ReadAsync(store.Id.Value, articleId: brie.Id.Value);
    Assert.NotNull(result);

    Assert.Equal(product.Sku?.Value, result.Sku);
    Assert.Equal(product.DisplayName.Value, result.DisplayName);
    Assert.Equal(product.Description?.Value, result.Description);
    Assert.Equal(product.Flags?.Value, result.Flags);
    Assert.Equal(product.UnitPrice?.Value, result.UnitPrice);
    Assert.Equal(product.UnitType?.Value, result.UnitType);

    Assert.Equal(brie.Id.Value, result.Article.Id);
    Assert.Equal(product.DepartmentNumber?.Value, result.Department?.Number);
    Assert.Equal(store.Id.Value, result.Store.Id);

    Assert.Equal(1, result.Version);
    Assert.Equal(ApplicationContext.Actor.Id, result.CreatedBy.Id);
    Assert.Equal(ApplicationContext.Actor.Id, result.UpdatedBy.Id);
    AssertAreNear(store.UpdatedOn, result.CreatedOn);
    Assert.Equal(result.CreatedOn, result.UpdatedOn);
  }

  [Fact(DisplayName = "ReadAsync: it should read the correct product by SKU.")]
  public async Task ReadAsync_it_should_read_the_correct_product_by_Sku()
  {
    Product? product = await productService.ReadAsync(store.Id.Value, sku: "notre-dame-brie");
    Assert.NotNull(product);
    Assert.Equal(brie.Id.Value, product.Article.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return null when no product are found.")]
  public async Task ReadAsync_it_should_return_null_when_no_product_are_found()
  {
    Assert.Null(await productService.ReadAsync(store.Id.Value, articleId: Guid.Empty.ToString(), sku: "test"));
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many products are found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_products_are_found()
  {
    DepartmentUnit department = store.Departments.Values.Single();
    SkuUnit sku = new("agro-camembert");
    FlagsUnit flags = new("MRJ");
    UnitPriceUnit unitPrice = new(6.99);
    UnitTypeUnit unitType = new("pc");
    ProductUnit product = new(camembert.Id, camembert.DisplayName, department.Number, sku, camembert.Description, flags, unitPrice, unitType);
    store.SetProduct(product, ApplicationContext.ActorId);
    await storeRepository.SaveAsync(store);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Product>>(
      async () => await productService.ReadAsync(store.Id.Value, brie.Id.Value, sku.Value)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "RemoveAsync: it should remove the found product.")]
  public async Task RemoveAsync_it_should_remove_the_found_product()
  {
    ArticleId articleId = store.Products.Keys.Single();

    AcceptedCommand command = await productService.RemoveAsync(store.Id.Value, articleId.Value);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);
  }

  [Fact(DisplayName = "RemoveAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task RemoveAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await productService.RemoveAsync(storeId.Value, camembert.Id.Value)
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "RemoveAsync: it should throw ProductNotFoundException when the product could not be found.")]
  public async Task RemoveAsync_it_should_throw_ProductNotFoundException_when_the_product_could_not_be_found()
  {
    var exception = await Assert.ThrowsAsync<ProductNotFoundException>(
      async () => await productService.RemoveAsync(store.Id.Value, camembert.Id.Value)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(camembert.Id, exception.ArticleId);
    Assert.Equal("ArticleId", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should save a new product.")]
  public async Task SaveAsync_it_should_save_a_new_product()
  {
    SaveProductPayload payload = new()
    {
      DepartmentNumber = "    ",
      Sku = "    ",
      DisplayName = $"  {camembert.DisplayName.Value}  ",
      Description = "    ",
      Flags = "    ",
      UnitType = "    "
    };

    AcceptedCommand command = await productService.SaveAsync(store.Id.Value, camembert.Id.Value, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ProductEntity? product = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Article!.AggregateId == camembert.Id.Value);
    Assert.NotNull(product);
    Assert.NotNull(product.Store);
    Assert.Equal(store.Id.Value, product.Store.AggregateId);
    Assert.Null(product.Department);
    Assert.NotNull(product.Article);
    Assert.Equal(camembert.Id.Value, product.Article.AggregateId);

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
    Assert.NotNull(brie.Gtin);
    SaveProductPayload payload = new()
    {
      DepartmentNumber = store.Departments.Values.Single().Number.Value,
      Sku = brie.Gtin.Value,
      DisplayName = $" {brie.DisplayName.Value} ",
      Description = " Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons. ",
      Flags = " MRJ ",
      UnitPrice = 6.99,
      UnitType = " pc "
    };

    AcceptedCommand command = await productService.SaveAsync(store.Id.Value, brie.Id.Value, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ProductEntity? product = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Article!.AggregateId == brie.Id.Value);
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

    AggregateId articleId = new("006740000026");
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
      async () => await productService.SaveAsync(storeId.Value, brie.Id.Value, payload)
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
      async () => await productService.SaveAsync(store.Id.Value, brie.Id.Value, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(payload.DepartmentNumber, exception.Number.Value);
    Assert.Equal(nameof(payload.DepartmentNumber), exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task SaveAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveProductPayload payload = new()
    {
      DisplayName = Faker.Random.String(DisplayNameUnit.MaximumLength + 1, minChar: 'A', maxChar: 'Z')
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(
      async () => await productService.SaveAsync(store.Id.Value, brie.Id.Value, payload)
    );
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchProductsPayload payload = new()
    {
      StoreId = store.Id.Value
    };
    payload.Id.Terms.Add(new SearchTerm(Guid.Empty.ToString()));

    SearchResults<Product> products = await productService.SearchAsync(payload);

    Assert.Empty(products.Results);
    Assert.Equal(0, products.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    ProductUnit brie = store.Products.Values.Single();

    GtinUnit okaBeerGtin = new("006740062322");
    ArticleAggregate okaBeerArticle = new(new DisplayNameUnit("OKA BEER 6X190G"), ApplicationContext.ActorId, new ArticleId(okaBeerGtin))
    {
      Gtin = okaBeerGtin
    };
    GtinUnit okaOriginalGtin = new("006740062517");
    ArticleAggregate okaOriginalArticle = new(new DisplayNameUnit("OKA FROMAG CLSSQ"), ApplicationContext.ActorId, new ArticleId(okaOriginalGtin))
    {
      Gtin = okaOriginalGtin
    };
    GtinUnit fetaGtin = new("006354999102");
    ArticleAggregate fetaArticle = new(new DisplayNameUnit("SAPUTO FROMAGE"), ApplicationContext.ActorId, new ArticleId(fetaGtin))
    {
      Gtin = fetaGtin
    };
    GtinUnit chevreGtin = new("006038387442");
    ArticleAggregate chevreArticle = new(new DisplayNameUnit("PC MB FRMAGE CHE"), ApplicationContext.ActorId, new ArticleId(chevreGtin))
    {
      Gtin = chevreGtin
    };
    GtinUnit patatesGtin = new("006148301467");
    ArticleAggregate patatesArticle = new(new DisplayNameUnit("POMMES TERRE"), ApplicationContext.ActorId, new ArticleId(patatesGtin))
    {
      Gtin = patatesGtin
    };
    await articleRepository.SaveAsync(new[] { okaBeerArticle, okaOriginalArticle, fetaArticle, chevreArticle, patatesArticle });

    DepartmentUnit department = store.Departments.Values.Single();

    DepartmentUnit fruitsLegumes = new(new DepartmentNumberUnit("27"), new DisplayNameUnit("FRUITS ET LEGUMES"));
    store.SetDepartment(fruitsLegumes);

    ProductUnit camembert = new(this.camembert.Id, this.camembert.DisplayName, department.Number, new SkuUnit("agropur-camembert"),
      this.camembert.Description, new FlagsUnit("MRJ"), new UnitPriceUnit(6.99), new UnitTypeUnit("pc"));
    store.SetProduct(camembert, ApplicationContext.ActorId);

    ProductUnit okaBeer = new(okaBeerArticle.Id, okaBeerArticle.DisplayName, department.Number);
    store.SetProduct(okaBeer, ApplicationContext.ActorId);

    ProductUnit okaOriginal = new(okaOriginalArticle.Id, okaOriginalArticle.DisplayName, department.Number);
    store.SetProduct(okaOriginal, ApplicationContext.ActorId);

    ProductUnit feta = new(fetaArticle.Id, fetaArticle.DisplayName, department.Number);
    store.SetProduct(feta, ApplicationContext.ActorId);

    ProductUnit chevre = new(chevreArticle.Id, chevreArticle.DisplayName, department.Number);
    store.SetProduct(chevre, ApplicationContext.ActorId);

    ProductUnit patates = new(patatesArticle.Id, patatesArticle.DisplayName, fruitsLegumes.Number);
    store.SetProduct(patates, ApplicationContext.ActorId);

    StoreNumberUnit number = new("08984");
    StoreAggregate otherStore = new(new DisplayNameUnit("Maxi Drummondville St-Joseph"), ApplicationContext.ActorId, new StoreId(banner, number))
    {
      Number = number
    };
    otherStore.SetDepartment(department, ApplicationContext.ActorId);

    ProductUnit product = new(this.brie.Id, this.brie.DisplayName, department.Number, new SkuUnit("brie-notre-dame"));
    otherStore.SetProduct(product, ApplicationContext.ActorId);

    await storeRepository.SaveAsync(new[] { store, otherStore });

    SearchProductsPayload payload = new()
    {
      StoreId = store.Id.Value,
      DepartmentNumber = department.Number.Value,
      Id = new TextSearch
      {
        Terms = new[]
        {
          brie.ArticleId.Value, camembert.ArticleId.Value, okaBeer.ArticleId.Value, okaOriginal.ArticleId.Value, feta.ArticleId.Value, patates.ArticleId.Value
        }.Select(id => new SearchTerm(id)).ToList(),
        Operator = SearchOperator.Or
      },
      Search = new TextSearch
      {
        Terms = new List<SearchTerm>
        {
          new("agropur-%"),
          new("fr%m%g%"),
          new("oka%"),
          new("POMMES TERRE"),
          new("%-brie")
        },
        Operator = SearchOperator.Or
      },
      Sort = new List<ProductSortOption>
      {
        new(ProductSort.Sku)
      },
      Skip = 1,
      Limit = 2
    };

    ProductUnit[] expected = new[] { brie, camembert, okaBeer, okaOriginal }
      .OrderBy(x => x.Sku?.Value).Skip(payload.Skip).Take(payload.Limit).ToArray();

    SearchResults<Product> products = await productService.SearchAsync(payload);
    Assert.Equal(4, products.Total);

    Assert.Equal(expected.Length, products.Results.Count);
    for (int i = 0; i < expected.Length; i++)
    {
      Assert.Equal(expected[i].ArticleId.Value, products.Results[i].Article.Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should throw AggregateNotFoundException when the store could not be found.")]
  public async Task UpdateAsync_it_should_throw_AggregateNotFoundException_when_the_store_could_not_be_found()
  {
    UpdateProductPayload payload = new();

    AggregateId storeId = new("MAXI-08984");
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<StoreAggregate>>(
      async () => await productService.UpdateAsync(storeId.Value, brie.Id.Value, payload)
    );
    Assert.Equal(storeId, exception.Id);
    Assert.Equal("StoreId", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw DepartmentNotFoundException when the department could not be found.")]
  public async Task UpdateAsync_it_should_throw_DepartmentNotFoundException_when_the_department_could_not_be_found()
  {
    UpdateProductPayload payload = new()
    {
      DepartmentNumber = new Modification<string>("1")
    };

    var exception = await Assert.ThrowsAsync<DepartmentNotFoundException>(
      async () => await productService.UpdateAsync(store.Id.Value, brie.Id.Value, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(payload.DepartmentNumber.Value, exception.Number.Value);
    Assert.Equal(nameof(payload.DepartmentNumber), exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ProductNotFoundException when the product could not be found.")]
  public async Task UpdateAsync_it_should_throw_ProductNotFoundException_when_the_product_could_not_be_found()
  {
    UpdateProductPayload payload = new();

    var exception = await Assert.ThrowsAsync<ProductNotFoundException>(
      async () => await productService.UpdateAsync(store.Id.Value, camembert.Id.Value, payload)
    );
    Assert.Equal(store.Id, exception.StoreId);
    Assert.Equal(camembert.Id, exception.ArticleId);
    Assert.Equal("ArticleId", exception.PropertyName);
  }

  [Fact(DisplayName = "UpdateAsync: it should throw ValidationException when the payload is not valid.")]
  public async Task UpdateAsync_it_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateProductPayload payload = new()
    {
      UnitPrice = new Modification<double?>(-9.99)
    };

    await Assert.ThrowsAsync<FluentValidation.ValidationException>(
      async () => await productService.UpdateAsync(store.Id.Value, brie.Id.Value, payload)
    );
  }

  [Fact(DisplayName = "UpdateAsync: it should update an existing product.")]
  public async Task UpdateAsync_it_should_update_an_existing_product()
  {
    UpdateProductPayload payload = new()
    {
      DepartmentNumber = new Modification<string>(store.Departments.Keys.Single().Value),
      Description = new Modification<string>("  Fromage à pâte molle alvéolée, le Brie Notre-Dame charme à chaque bouchée. Sa saveur de crème est relevée par un délicieux arôme de champignons.  "),
      Flags = new Modification<string>(" MRJ "),
      UnitPrice = new Modification<double?>(6.99),
      UnitType = new Modification<string>(" pc ")
    };

    AcceptedCommand command = await productService.UpdateAsync(store.Id.Value, brie.Id.Value, payload);

    Assert.Equal(store.Id.Value, command.AggregateId);
    Assert.True(command.AggregateVersion > store.Version);
    Assert.Equal(ApplicationContext.Actor, command.Actor);
    AssertIsNear(command.Timestamp);

    ProductEntity? product = await FakturContext.Products.AsNoTracking()
      .Include(x => x.Article)
      .Include(x => x.Department)
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == store.Id.Value && x.Article!.AggregateId == brie.Id.Value);
    Assert.NotNull(product);
    Assert.NotNull(product.Store);
    Assert.Equal(store.Id.Value, product.Store.AggregateId);
    Assert.NotNull(product.Article);
    Assert.Equal(brie.Id.Value, product.Article.AggregateId);
    Assert.NotNull(product.Department);
    Assert.NotNull(payload.DepartmentNumber.Value);
    Assert.Equal(int.Parse(payload.DepartmentNumber.Value), product.Department.NumberNormalized);

    Assert.Equal(payload.Description.Value?.Trim(), product.Description);
    Assert.Equal(payload.Flags.Value?.Trim(), product.Flags);
    Assert.Equal(payload.UnitPrice.Value, product.UnitPrice);
    Assert.Equal(payload.UnitType.Value?.Trim(), product.UnitType);

    Assert.Equal(2, product.Version);
    Assert.Equal(ApplicationContext.Actor.Id, product.CreatedBy);
    Assert.Equal(ApplicationContext.Actor.Id, product.UpdatedBy);
    AssertIsNear(AsUniversalTime(product.UpdatedOn));
    Assert.True(product.UpdatedOn > product.CreatedOn);
  }
}
