using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Banners;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository articleRepository;

  public DeleteBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(DeleteBannerCommand command, CancellationToken cancellationToken)
  {
    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    article.Delete(applicationContext.ActorId);
    // TODO(fpion): remove banner from referencing stores

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}
