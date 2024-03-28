using Faktur.Application.Articles;
using Faktur.Application.Departments;
using Faktur.Application.Products.Validators;
using Faktur.Application.Stores;
using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class CreateOrReplaceProductCommandHandler : IRequestHandler<CreateOrReplaceProductCommand, CreateOrReplaceProductResult>
{
  private readonly IArticleRepository _articleRepository;
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;
  private readonly IPublisher _publisher;
  private readonly IStoreRepository _storeRepository;

  public CreateOrReplaceProductCommandHandler(IArticleRepository articleRepository, IProductQuerier productQuerier,
    IProductRepository productRepository, IPublisher publisher, IStoreRepository storeRepository)
  {
    _articleRepository = articleRepository;
    _productQuerier = productQuerier;
    _productRepository = productRepository;
    _publisher = publisher;
    _storeRepository = storeRepository;
  }

  public async Task<CreateOrReplaceProductResult> Handle(CreateOrReplaceProductCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceProductPayload payload = command.Payload;
    new CreateOrReplaceProductValidator().ValidateAndThrow(payload);

    StoreAggregate store = await _storeRepository.LoadAsync(command.StoreId, cancellationToken)
      ?? throw new StoreNotFoundException(command.StoreId, nameof(command.StoreId));

    bool isCreated = false;
    ProductAggregate? product = await _productRepository.LoadAsync(command.StoreId, command.ArticleId, cancellationToken);
    if (product == null)
    {
      isCreated = true;

      ArticleAggregate article = await _articleRepository.LoadAsync(command.ArticleId, cancellationToken)
        ?? throw new ArticleNotFoundException(command.ArticleId, nameof(command.ArticleId));

      product = new(store, article, command.ActorId);
    }

    NumberUnit? departmentNumber = NumberUnit.TryCreate(payload.DepartmentNumber);
    if (departmentNumber != null && !store.HasDepartment(departmentNumber))
    {
      throw new DepartmentNotFoundException(store, departmentNumber, nameof(payload.DepartmentNumber));
    }

    ProductAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _productRepository.LoadAsync(product.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null || departmentNumber != reference.DepartmentNumber)
    {
      product.DepartmentNumber = departmentNumber;
    }

    SkuUnit? sku = SkuUnit.TryCreate(payload.Sku);
    if (reference == null || sku != reference.Sku)
    {
      product.Sku = sku;
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      product.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      product.Description = description;
    }

    FlagsUnit? flags = FlagsUnit.TryCreate(payload.Flags);
    if (reference == null || flags != reference.Flags)
    {
      product.Flags = flags;
    }

    if (reference == null || payload.UnitPrice != reference.UnitPrice)
    {
      product.UnitPrice = payload.UnitPrice;
    }
    if (reference == null || payload.UnitType != reference.UnitType)
    {
      product.UnitType = payload.UnitType;
    }

    product.Update(command.ActorId);

    await _publisher.Publish(new SaveProductCommand(product), cancellationToken);

    Product result = await _productQuerier.ReadAsync(product, cancellationToken);
    return new CreateOrReplaceProductResult(isCreated, result);
  }
}
