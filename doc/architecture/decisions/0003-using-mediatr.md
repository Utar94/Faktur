# 3. Using MediatR

Date: 2023-09-09

## Status

Accepted

## Context

We need a tool to decouple Command/Query handlers from their definition. We also need a tool to
execute event handlers synchronously.

## Decision

We will be using [MediatR](https://github.com/jbogard/MediatR) NuGet packages.

## Consequences

Decoupling between handlers and Command/query or events will be made easily, but more obscure since
we won't see code references to the handlers.
