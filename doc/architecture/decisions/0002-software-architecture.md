# 2. Software Architecture

Date: 2023-09-09

## Status

Accepted

## Context

We need to develop an identity provider (IdP) that is highly scalable and auditable. We also want to
test our code and make it understandable to a domain expert, at a certain extent.

## Decision

We will implement the application using the following concepts/patterns:

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Command and Query Responsibility Segregation](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design)
- [Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)

## Consequences

Our application may take longer to develop and maintain, but its code will be separated in well
defined layer and easily testable, understandable by a domain expert, highly scalable and auditable.
