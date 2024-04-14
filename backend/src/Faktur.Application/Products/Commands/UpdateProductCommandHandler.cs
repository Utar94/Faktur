using Faktur.Application.Departments;
using Faktur.Application.Products.Validators;
using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product?>
{
  private readonly IProductQuerier _productQuerier;
  private readonly IProductRepository _productRepository;
  private readonly ISender _sender;
  private readonly IStoreRepository _storeRepository;

  public UpdateProductCommandHandler(IProductQuerier productQuerier, IProductRepository productRepository, ISender sender, IStoreRepository storeRepository)
  {
    _productQuerier = productQuerier;
    _productRepository = productRepository;
    _sender = sender;
    _storeRepository = storeRepository;
  }

  public async Task<Product?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
  {
    UpdateProductPayload payload = command.Payload;
    new UpdateProductValidator().ValidateAndThrow(payload);

    ProductAggregate? product = await _productRepository.LoadAsync(command.Id, cancellationToken);
    if (product == null)
    {
      return null;
    }

    if (payload.DepartmentNumber != null)
    {
      NumberUnit? departmentNumber = NumberUnit.TryCreate(payload.DepartmentNumber.Value);
      if (departmentNumber != null)
      {
        StoreAggregate store = await _storeRepository.LoadAsync(product.StoreId, cancellationToken)
          ?? throw new InvalidOperationException($"The store 'Id={product.StoreId.Value}' could not be found.");
        if (!store.HasDepartment(departmentNumber))
        {
          throw new DepartmentNotFoundException(store, departmentNumber, nameof(payload.DepartmentNumber));
        }
      }
      product.DepartmentNumber = departmentNumber;
    }

    if (payload.Sku != null)
    {
      product.Sku = SkuUnit.TryCreate(payload.Sku.Value);
    }
    if (payload.DisplayName != null)
    {
      product.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      product.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    if (payload.Flags != null)
    {
      product.Flags = FlagsUnit.TryCreate(payload.Flags.Value);
    }

    if (payload.UnitPrice != null)
    {
      product.UnitPrice = payload.UnitPrice.Value;
    }
    if (payload.UnitType != null)
    {
      product.UnitType = payload.UnitType.Value;
    }

    product.Update(command.ActorId);

    await _sender.Send(new SaveProductCommand(product), cancellationToken);

    return await _productQuerier.ReadAsync(product, cancellationToken);
  }
}
