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

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public UpdateProductCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
  {
    UpdateProductPayload payload = command.Payload;
    new UpdateProductPayloadValidator().ValidateAndThrow(payload);

    ArticleId articleId = ArticleId.Parse(command.ArticleId, nameof(command.ArticleId));

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    if (!store.Products.TryGetValue(articleId, out ProductUnit? product))
    {
      throw new ProductNotFoundException(store, articleId, nameof(command.ArticleId));
    }

    DepartmentNumberUnit? departmentNumber = product.DepartmentNumber;
    if (payload.DepartmentNumber != null)
    {
      departmentNumber = DepartmentNumberUnit.TryCreate(payload.DepartmentNumber.Value);
      if (departmentNumber != null && !store.Departments.ContainsKey(departmentNumber))
      {
        throw new DepartmentNotFoundException(store, departmentNumber, nameof(payload.DepartmentNumber));
      }
    }

    SkuUnit? sku = product.Sku;
    if (payload.Sku != null)
    {
      sku = SkuUnit.TryCreate(payload.Sku.Value);
    }
    DisplayNameUnit displayName = product.DisplayName;
    if (payload.DisplayName != null)
    {
      displayName = new(payload.DisplayName);
    }
    DescriptionUnit? description = product.Description;
    if (payload.Description != null)
    {
      description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    FlagsUnit? flags = product.Flags;
    if (payload.Flags != null)
    {
      flags = FlagsUnit.TryCreate(payload.Flags.Value);
    }

    UnitPriceUnit? unitPrice = product.UnitPrice;
    if (payload.UnitPrice != null)
    {
      unitPrice = UnitPriceUnit.TryCreate(payload.UnitPrice.Value);
    }
    UnitTypeUnit? unitType = product.UnitType;
    if (payload.UnitType != null)
    {
      unitType = UnitTypeUnit.TryCreate(payload.UnitType.Value);
    }

    product = new(articleId, displayName, departmentNumber, sku, description, flags, unitPrice, unitType);
    store.SetProduct(product, applicationContext.ActorId);

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
