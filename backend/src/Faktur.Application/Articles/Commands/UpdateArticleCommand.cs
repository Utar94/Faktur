using Faktur.Application.Activities;
using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

public record UpdateArticleCommand(Guid Id, UpdateArticlePayload Payload) : Activity, IRequest<Article?>;
