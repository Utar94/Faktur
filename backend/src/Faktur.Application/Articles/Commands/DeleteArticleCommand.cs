using Faktur.Application.Activities;
using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

public record DeleteArticleCommand(Guid Id) : Activity, IRequest<Article?>;
