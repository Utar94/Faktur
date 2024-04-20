using Faktur.Application.Articles;
using Faktur.Application.Products;
using Faktur.Application.Receipts.Parsing;
using Faktur.Application.Receipts.Validators;
using Faktur.Application.Stores;
using Faktur.Contracts.Articles;
using Faktur.Contracts.Products;
using Faktur.Contracts.Receipts;
using Faktur.Contracts.Stores;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Receipts.Commands;

internal class ImportReceiptCommandHandler : IRequestHandler<ImportReceiptCommand, Receipt>
{
  private readonly IArticleQuerier _articleQuerier;
  private readonly IArticleRepository _articleRepository;
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;
  private readonly IReceiptParser _receiptParser;
  private readonly IReceiptQuerier _receiptQuerier;
  private readonly IReceiptRepository _receiptRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ITaxRepository _taxRepository;

  public ImportReceiptCommandHandler(IArticleQuerier articleQuerier, IArticleRepository articleRepository, IProductQuerier productQuerier, IProductRepository productRepository,
    IReceiptParser receiptParser, IReceiptQuerier receiptQuerier, IReceiptRepository receiptRepository, IStoreRepository storeRepository, ITaxRepository taxRepository)
  {
    _articleQuerier = articleQuerier;
    _articleRepository = articleRepository;
    _productQuerier = productQuerier;
    _productRepository = productRepository;
    _receiptParser = receiptParser;
    _receiptQuerier = receiptQuerier;
    _receiptRepository = receiptRepository;
    _storeRepository = storeRepository;
    _taxRepository = taxRepository;
  }

  public async Task<Receipt> Handle(ImportReceiptCommand command, CancellationToken cancellationToken)
  {
    ImportReceiptPayload payload = command.Payload;
    new ImportReceiptValidator().ValidateAndThrow(payload);

    StoreAggregate store = await _storeRepository.LoadAsync(payload.StoreId, cancellationToken)
      ?? throw new StoreNotFoundException(payload.StoreId, nameof(payload.StoreId));

    IEnumerable<ReceiptItemUnit> items = [];
    if (!string.IsNullOrWhiteSpace(payload.Lines))
    {
      LocaleUnit? locale = LocaleUnit.TryCreate(payload.Locale);
      items = await _receiptParser.ExecuteAsync(payload.Lines, nameof(payload.Lines), locale, cancellationToken);

      int capacity = items.Count();
      List<ArticleAggregate> createdArticles = new(capacity);
      List<ProductAggregate> createdProducts = new(capacity);

      Dictionary<long, Article> articles = await LoadArticlesAsync(cancellationToken);
      Dictionary<string, Product> products = await LoadProductsAsync(store, cancellationToken);
      Store currentStore = ToStore(store);

      foreach (ReceiptItemUnit item in items)
      {
        if (item.DepartmentNumber != null && item.Department != null && !store.HasDepartment(item.DepartmentNumber))
        {
          store.SetDepartment(item.DepartmentNumber, item.Department, command.ActorId);
        }

        Article? foundArticle = null;
        Product? foundProduct = null;

        if (item.Gtin != null)
        {
          if (articles.TryGetValue(item.Gtin.NormalizedValue, out foundArticle))
          {
            _ = products.TryGetValue(GetArticleIdKey(foundArticle), out foundProduct);
          }
        }
        else if (item.Sku != null)
        {
          if (products.TryGetValue(GetSkuKey(item.Sku), out foundProduct))
          {
            foundArticle = foundProduct.Article;
          }
        }

        ArticleAggregate? article = null;
        if (foundArticle == null)
        {
          article = CreateArticle(item, command.ActorId);
          createdArticles.Add(article);

          foundArticle = ToArticle(article);
          if (foundArticle.Gtin != null)
          {
            articles[long.Parse(foundArticle.Gtin)] = foundArticle;
          }
        }
        if (foundProduct == null)
        {
          article ??= await _articleRepository.LoadAsync(foundArticle.Id, cancellationToken)
            ?? throw new InvalidOperationException($"The article 'Id={foundArticle.Id}' could not be found.");

          ProductAggregate product = CreateProduct(store, article, item, command.ActorId);
          createdProducts.Add(product);

          foundProduct = ToProduct(foundArticle, currentStore, product);
          products[GetArticleIdKey(foundProduct.Article)] = foundProduct;
          if (foundProduct.Sku != null)
          {
            products[GetSkuKey(foundProduct.Sku)] = foundProduct;
          }
        }
      }

      await _storeRepository.SaveAsync(store, cancellationToken);
      await _articleRepository.SaveAsync(createdArticles, cancellationToken);
      await _productRepository.SaveAsync(createdProducts, cancellationToken);
    }

    NumberUnit? number = NumberUnit.TryCreate(payload.Number);
    IEnumerable<TaxAggregate> taxes = await _taxRepository.LoadAsync(cancellationToken);
    ReceiptAggregate receipt = ReceiptAggregate.Import(store, payload.IssuedOn, number, items, taxes, command.ActorId);

    receipt.Calculate(taxes, command.ActorId);
    await _receiptRepository.SaveAsync(receipt, cancellationToken);

    return await _receiptQuerier.ReadAsync(receipt, cancellationToken);
  }

  private async Task<Dictionary<long, Article>> LoadArticlesAsync(CancellationToken cancellationToken)
  {
    SearchResults<Article> results = await _articleQuerier.SearchAsync(new SearchArticlesPayload(), cancellationToken);

    Dictionary<long, Article> articles = new(capacity: results.Items.Count);
    foreach (Article article in results.Items)
    {
      if (article.Gtin != null)
      {
        articles[long.Parse(article.Gtin)] = article;
      }
    }
    return articles;
  }
  private async Task<Dictionary<string, Product>> LoadProductsAsync(StoreAggregate store, CancellationToken cancellationToken)
  {
    SearchResults<Product> results = await _productQuerier.SearchAsync(new SearchProductsPayload(store.Id.ToGuid()), cancellationToken);

    Dictionary<string, Product> products = new(capacity: 2 * results.Items.Count);
    foreach (Product product in results.Items)
    {
      products[GetArticleIdKey(product.Article)] = product;
      if (product.Sku != null)
      {
        products[GetSkuKey(product.Sku)] = product;
      }
    }
    return products;
  }
  private static string GetArticleIdKey(Article article) => $"ArticleId:{article.Id}";
  private static string GetSkuKey(SkuUnit sku) => GetSkuKey(sku.Value);
  private static string GetSkuKey(string sku) => $"Sku:{sku.ToUpper()}";

  private static ArticleAggregate CreateArticle(ReceiptItemUnit item, ActorId actorId)
  {
    ArticleAggregate article = new(item.Label, actorId)
    {
      Gtin = item.Gtin
    };
    article.Update(actorId);
    return article;
  }
  private static ProductAggregate CreateProduct(StoreAggregate store, ArticleAggregate article, ReceiptItemUnit item, ActorId actorId)
  {
    ProductAggregate product = new(store, article, actorId)
    {
      DepartmentNumber = item.DepartmentNumber,
      Sku = item.Sku,
      DisplayName = item.Label,
      Flags = item.Flags,
      UnitPrice = item.UnitPrice
    };
    product.Update(actorId);
    return product;
  }

  private static Article ToArticle(ArticleAggregate article) => new(article.DisplayName.Value)
  {
    Id = article.Id.ToGuid(),
    Gtin = article.Gtin?.Value
  };
  private static Product ToProduct(Article article, Store store, ProductAggregate source) => new(article, store)
  {
    Sku = source.Sku?.Value
  };
  private static Store ToStore(StoreAggregate source) => new(source.DisplayName.Value);
}
