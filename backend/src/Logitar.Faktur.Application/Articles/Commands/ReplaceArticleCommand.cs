using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Commands;

internal record ReplaceArticleCommand(string Id, ReplaceArticlePayload Payload, long? Version) : IRequest<AcceptedCommand>;
