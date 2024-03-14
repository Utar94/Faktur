﻿using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Application.Products.Commands;

internal record DeleteStoreProductsCommand(ActorId ActorId, StoreAggregate Store) : INotification;
