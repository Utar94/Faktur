using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Application.Products.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchProductsQueryTests : IntegrationTests
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductRepository _productRepository;
  private readonly IStoreRepository _storeRepository;

  private readonly ArticleAggregate _article;
  private readonly StoreAggregate _store;

  public SearchProductsQueryTests() : base()
  {
    _articleRepository = ServiceProvider.GetRequiredService<IArticleRepository>();
    _productRepository = ServiceProvider.GetRequiredService<IProductRepository>();
    _storeRepository = ServiceProvider.GetRequiredService<IStoreRepository>();

    _article = new(new DisplayNameUnit("POIVRONS VERTS"), ActorId);
    _store = new(new DisplayNameUnit("Maxi Drummondville"), ActorId)
    {
      Number = new NumberUnit("08872")
    };
    _store.Update(ActorId);
    _store.SetDepartment(new NumberUnit("27"), new DepartmentUnit(new DisplayNameUnit("FRUITS ET LEGUMES")), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [FakturDb.Products.Table, FakturDb.Stores.Table, FakturDb.Articles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _articleRepository.SaveAsync(_article);
    await _storeRepository.SaveAsync(_store);
  }

  [Fact(DisplayName = "It should return empty results when none were found.")]
  public async Task It_should_return_empty_results_when_none_were_found()
  {
    SearchProductsPayload payload = new();
    SearchProductsQuery query = new(payload);
    SearchResults<Product> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    NumberUnit departmentNumber = new("27");

    StoreAggregate otherStore = new(new DisplayNameUnit("Maxi Beloeil"), ActorId);
    otherStore.SetDepartment(departmentNumber, new(new DisplayNameUnit("FRUITS ET LEGUMES")), ActorId);
    await _storeRepository.SaveAsync(otherStore);

    ProductAggregate otherStoreProduct = new(otherStore, _article, ActorId)
    {
      DepartmentNumber = departmentNumber,
      Sku = new SkuUnit("3120"),
      DisplayName = new DisplayNameUnit("Poivrons verts"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 7.69m,
      UnitType = UnitType.Kg
    };
    otherStoreProduct.Update(ActorId);

    ArticleAggregate peanuts = new(new DisplayNameUnit("PLANTERS PNUT"), ActorId)
    {
      Gtin = new GtinUnit("05871699301")
    };
    peanuts.Update(ActorId);
    ProductAggregate notInDepartment = new(_store, peanuts, ActorId)
    {
      DisplayName = new DisplayNameUnit("Planters peanuts"),
      Flags = new FlagsUnit("FPMRJ"),
      UnitPrice = 30.15m,
      UnitType = UnitType.Kg
    };
    notInDepartment.Update(ActorId);

    ArticleAggregate bananas = new(new DisplayNameUnit("BANANES"), ActorId);
    ProductAggregate notInIds = new(_store, bananas, ActorId)
    {
      DepartmentNumber = departmentNumber,
      Sku = new SkuUnit("4011"),
      DisplayName = new DisplayNameUnit("Bananes"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 1.52m,
      UnitType = UnitType.Kg
    };
    notInIds.Update(ActorId);

    ArticleAggregate tomatoes = new(new DisplayNameUnit("TOMATES GRAPPE"), ActorId);
    ProductAggregate notMatching = new(_store, tomatoes, ActorId)
    {
      DepartmentNumber = departmentNumber,
      Sku = new SkuUnit("4664"),
      DisplayName = new DisplayNameUnit("Tomates en grappe"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 3.17m,
      UnitType = UnitType.Kg
    };
    notMatching.Update(ActorId);

    ArticleAggregate carrots = new(new DisplayNameUnit("DM CAROTTES 3 LB"), ActorId)
    {
      Gtin = new GtinUnit("06148301460")
    };
    carrots.Update(ActorId);
    ProductAggregate notUnitType = new(_store, carrots, ActorId)
    {
      DepartmentNumber = departmentNumber,
      DisplayName = new DisplayNameUnit("Carottes"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 2.99m
    };
    notUnitType.Update(ActorId);

    ArticleAggregate rutabagas = new(new DisplayNameUnit("RUTABAGAS"), ActorId);
    ProductAggregate product = new(_store, rutabagas, ActorId)
    {
      DepartmentNumber = departmentNumber,
      Sku = new SkuUnit("4747"),
      DisplayName = new DisplayNameUnit("Rutabagas"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 2.18m,
      UnitType = UnitType.Kg
    };
    product.Update(ActorId);

    ProductAggregate expected = new(_store, _article, ActorId)
    {
      DepartmentNumber = departmentNumber,
      Sku = new SkuUnit("3120"),
      DisplayName = new DisplayNameUnit("Poivrons verts"),
      Flags = new FlagsUnit("MRJ"),
      UnitPrice = 7.69m,
      UnitType = UnitType.Kg
    };
    expected.Update(ActorId);

    await _articleRepository.SaveAsync([peanuts, bananas, tomatoes, carrots, rutabagas]);
    await _productRepository.SaveAsync([otherStoreProduct, notInDepartment, notInIds, notMatching, notUnitType, product, expected]);

    SearchProductsPayload payload = new(_store.Id.ToGuid())
    {
      DepartmentNumber = "27",
      UnitType = UnitType.Kg,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%carotte%"));
    payload.Search.Terms.Add(new SearchTerm("%plant%"));
    payload.Search.Terms.Add(new SearchTerm("%poivron%"));
    payload.Search.Terms.Add(new SearchTerm("40__"));
    payload.Search.Terms.Add(new SearchTerm("4747"));
    payload.Sort.Add(new ProductSortOption(ProductSort.UnitPrice));

    IEnumerable<ProductAggregate> allProducts = await _productRepository.LoadAsync();
    payload.Ids.AddRange(allProducts.Select(article => article.Id.ToGuid()));
    payload.Ids.Add(Guid.Empty);
    payload.Ids.Remove(notInIds.Id.ToGuid());

    SearchProductsQuery query = new(payload);
    SearchResults<Product> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Product result = Assert.Single(results.Items);
    Assert.Equal(expected.Id.ToGuid(), result.Id);
  }
}
