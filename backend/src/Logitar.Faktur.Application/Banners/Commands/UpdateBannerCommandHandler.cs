﻿using FluentValidation;
using Logitar.Faktur.Application.Banners.Validators;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Application.Banners.Commands;

internal class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, AcceptedCommand>
{
  private readonly IApplicationContext applicationContext;
  private readonly IBannerRepository articleRepository;

  public UpdateBannerCommandHandler(IApplicationContext applicationContext, IBannerRepository articleRepository)
  {
    this.applicationContext = applicationContext;
    this.articleRepository = articleRepository;
  }

  public async Task<AcceptedCommand> Handle(UpdateBannerCommand command, CancellationToken cancellationToken)
  {
    UpdateBannerPayload payload = command.Payload;
    new UpdateBannerPayloadValidator().ValidateAndThrow(payload);

    BannerId id = BannerId.Parse(command.Id, nameof(command.Id));
    BannerAggregate article = await articleRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<BannerAggregate>(id.AggregateId, nameof(command.Id));

    if (!string.IsNullOrWhiteSpace(payload.DisplayName))
    {
      article.DisplayName = new DisplayNameUnit(payload.DisplayName);
    }
    if (payload.Description != null)
    {
      article.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    article.Update(applicationContext.ActorId);

    await articleRepository.SaveAsync(article, cancellationToken);

    return applicationContext.AcceptCommand(article);
  }
}