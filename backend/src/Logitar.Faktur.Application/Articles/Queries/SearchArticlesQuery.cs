using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Queries;

internal record SearchArticlesQuery(SearchArticlesPayload Payload) : IRequest<SearchResults<Article>>;
