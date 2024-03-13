using Faktur.Application.Activities;
using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

public record ReplaceArticleCommand(Guid Id, ReplaceArticlePayload Payload, long? Version) : Activity, IRequest<Article?>;
