# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Implemented a relocation tool.

## [2.0.2] - 2024-10-23

### Fixed

- Changed `POSTGRESQLCONNSTR_Portal` to `POSTGRESQLCONNSTR_Faktur`.
- Removed version from `docker-compose.yml`.

## [2.0.1] - 2024-10-22

### Added

- Implemented publication to DockerHub.

### Fixed

- Fixed NuGet vulnerability.

## [2.0.0] - 2024-04-20

### Added

- Implemented tax management.
- Category browser persistence.
- Clean Architecture, Command and Query Responsibility Segregation (CQRS), Event Sourcing and Domain-Driven Design (DDD).
- Unit and Integration tests.
- Store contact information.

### Changed

- Reimplemented article management.
- Reimplemented banner management.
- Reimplemented store management.
- Reimplemented department management.
- Reimplemented product management.
- Reimplemented receipt management.
- Authentication with Logitar Portal.
- Updated frontend to Vue 3.

## [1.7.2] - 2023-09-22

### Fixed

- Fixed CHANGELOG.

## [1.7.1] - 2022-03-23

### Added

- Implemented article management.
- Implemented banner management.
- Implemented store management.
- Implemented department management.
- Implemented product management.
- Implemented receipt management.

[unreleased]: https://github.com/Utar94/Faktur/compare/v2.0.2...HEAD
[2.0.2]: https://github.com/Utar94/Faktur/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/Utar94/Faktur/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/Utar94/Faktur/compare/v1.7.2...v2.0.0
[1.7.2]: https://github.com/Utar94/Faktur/compare/v1.7.1...v1.7.2
[1.7.1]: https://github.com/Utar94/Faktur/releases/tag/v1.7.1
