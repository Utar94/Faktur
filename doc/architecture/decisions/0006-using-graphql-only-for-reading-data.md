# 6. Using GraphQL only for reading data

Date: 2023-09-09

## Status

Accepted

## Context

We need Web interfaces that are human readable and convenient to use.

## Decision

We will be implementing [GraphQL](https://graphql.org/) queries to expose data to authorized actors.
We will only be exposing queries, no mutation nor subscription.

## Consequences

Thanks to its verbosity and available user interfaces, the GraphQL Web interface will be providing
an humanly readable way to query data.
