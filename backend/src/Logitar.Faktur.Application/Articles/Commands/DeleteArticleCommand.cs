using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Commands;

internal record DeleteArticleCommand(string Id) : IRequest<AcceptedCommand>;
