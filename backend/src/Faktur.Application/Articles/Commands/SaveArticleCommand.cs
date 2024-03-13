using Faktur.Domain.Articles;
using MediatR;

namespace Faktur.Application.Articles.Commands;

internal record SaveArticleCommand(ArticleAggregate Article) : INotification;
