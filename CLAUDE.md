# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FinnhubSdk is a .NET client library for the Finnhub API, providing both REST and WebSocket support for accessing financial market data.

## Project Status

Project structure: `src/FinnhubSdk`, `tests/FinnhubSdk.Tests.Unit`. Uses centralized package management (Directory.Packages.props) and common build properties (Directory.Build.props). SDK implementation complete (Phases 1-5 done), CI/CD setup in progress (Phase 6).

**Note**: This project does NOT include integration tests. Most Finnhub API endpoints require a paid API key for proper testing, and free tier rate limits make automated integration testing impractical. Do not add integration tests.

## Technology Stack

.NET 9.0, C# (latest), targeting Finnhub REST API and WebSocket API. Nullable reference types enabled, implicit usings enabled, TreatWarningsAsErrors enabled, Source Link integration included.

## Development Setup

Standard commands: `dotnet build`, `dotnet test`, `dotnet restore`, `dotnet pack`. Package versions managed centrally in Directory.Packages.props.

## Coding Standards

### XML Documentation (MANDATORY)

All public classes, methods, and properties MUST have XML documentation with `<summary>`, `<param>`, `<returns>`, and `<exception>` tags as applicable. The CS1591 warning is currently suppressed but should be removed once documentation is complete.

### Use TimeSpan for Time Durations (IMPORTANT)

Always use `TimeSpan` for time durations. Never use `int timeoutSeconds` or similar - use `TimeSpan timeout` instead. This provides type safety, flexibility, and clarity.

### General Guidelines

- Handle nullability properly (nullable reference types enabled)
- Follow C# naming conventions (PascalCase for public, camelCase for private)
- Write unit tests for all public APIs
- Keep methods focused and single-purpose

## Documentation Requirements

### README.md Maintenance (CRITICAL)

**The README.md must always reflect the current state of the library.** When you add, modify, or remove features, update the README in the same commit. All code samples must work with the current version - verify they compile before committing. Follow typical GitHub README structure: description, installation, quick start example, key features, configuration, links. Keep it concise and readable.

## Architecture (To Be Implemented)

The SDK will need to support:
- REST API client for synchronous data requests
- WebSocket client for real-time data streaming
- Authentication with Finnhub API keys
- Serialization/deserialization of Finnhub API responses
