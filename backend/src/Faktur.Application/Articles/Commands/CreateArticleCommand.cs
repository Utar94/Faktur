using Faktur.Application.Activities;
using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

public record CreateArticleCommand(CreateArticlePayload Payload) : Activity, IRequest<Article>;
