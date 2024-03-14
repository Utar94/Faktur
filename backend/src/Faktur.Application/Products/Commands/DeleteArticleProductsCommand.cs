using Faktur.Domain.Articles;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal record DeleteArticleProductsCommand(ActorId ActorId, ArticleAggregate Article) : INotification;
