﻿using Logitar.Faktur.Contracts.Articles;
using MediatR;

namespace Logitar.Faktur.Application.Articles.Queries;

internal record ReadArticleQuery(string Id) : IRequest<Article?>;
