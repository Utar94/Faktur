using Faktur.Contracts.Articles;
using MediatR;

namespace Faktur.Application.Articles.Queries;

public record ReadArticleQuery(Guid? Id, string? Gtin) : IRequest<Article?>;
