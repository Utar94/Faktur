using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Products.Commands;

internal class SaveProductCommandHandler : IRequestHandler<SaveProductCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IArticleRepository articleRepository;
  private readonly IStoreRepository storeRepository;

  public SaveProductCommandHandler(IApplicationContext applicationContext, IArticleRepository articleRepository, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(SaveProductCommand command, CancellationToken cancellationToken)
  {
    SaveProductPayload payload = command.Payload; // TODO(fpion): validate

    ArticleId articleId = ArticleId.Parse(command.StoreId, nameof(command.StoreId));
    ArticleAggregate article = await articleRepository.LoadAsync(articleId, cancellationToken)
      ?? throw new AggregateNotFoundException<ArticleAggregate>(articleId.AggregateId, nameof(command.ArticleId));

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    DepartmentNumberUnit? departmentNumber = DepartmentNumberUnit.TryCreate(payload.DepartmentNumber);
    if (departmentNumber != null && !store.Departments.ContainsKey(departmentNumber))
    {
      throw new DepartmentNotFoundException(store, departmentNumber, nameof(payload.DepartmentNumber));
    }

    // TODO(fpion): Sku
    DisplayNameUnit displayName = new(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    ProductUnit product = new(article.Id, displayName, description);
    store.SetProduct(product);

    // TODO(fpion): Flags

    // TODO(fpion): UnitPrice
    // TODO(fpion): UnitType

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
