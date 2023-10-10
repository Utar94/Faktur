using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Products.Commands;

internal class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IStoreRepository storeRepository;

  public RemoveProductCommandHandler(IApplicationContext applicationContext, IStoreRepository storeRepository)
  {
    this.applicationContext = applicationContext;
    this.storeRepository = storeRepository;
  }

  public async Task<AcceptedCommand> Handle(RemoveProductCommand command, CancellationToken cancellationToken)
  {
    ArticleId articleId = ArticleId.Parse(command.ArticleId, nameof(command.ArticleId));

    StoreId storeId = StoreId.Parse(command.StoreId, nameof(command.StoreId));
    StoreAggregate store = await storeRepository.LoadAsync(storeId, cancellationToken)
      ?? throw new AggregateNotFoundException<StoreAggregate>(storeId.AggregateId, nameof(command.StoreId));

    if (!store.RemoveProduct(articleId, applicationContext.ActorId))
    {
      throw new ProductNotFoundException(store, articleId, nameof(command.ArticleId));
    }

    await storeRepository.SaveAsync(store, cancellationToken);

    return applicationContext.AcceptCommand(store);
  }
}
