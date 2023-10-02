using FluentValidation;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Application.Products.Validators;
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
    SaveProductPayload payload = command.Payload;
    new SaveProductPayloadValidator().ValidateAndThrow(payload);

    ArticleId articleId = ArticleId.Parse(command.ArticleId, nameof(command.ArticleId));
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

    SkuUnit? sku = SkuUnit.TryCreate(payload.Sku);
    DisplayNameUnit displayName = new(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    FlagsUnit? flags = FlagsUnit.TryCreate(payload.Flags);
    UnitPriceUnit? unitPrice = UnitPriceUnit.TryCreate(payload.UnitPrice);
    UnitTypeUnit? unitType = UnitTypeUnit.TryCreate(payload.UnitType);
    ProductUnit product = new(article.Id, displayName, departmentNumber, sku, description, flags, unitPrice, unitType);
    store.SetProduct(product, applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
