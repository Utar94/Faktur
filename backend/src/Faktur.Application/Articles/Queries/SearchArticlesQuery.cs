using Faktur.Contracts.Articles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Articles.Queries;

public record SearchArticlesQuery(SearchArticlesPayload Payload) : IRequest<SearchResults<Article>>;
