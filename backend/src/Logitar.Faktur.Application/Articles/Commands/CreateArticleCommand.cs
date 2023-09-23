using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Articles;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Commands;

internal record CreateArticleCommand(CreateArticlePayload Payload) : IRequest<AcceptedCommand>;
