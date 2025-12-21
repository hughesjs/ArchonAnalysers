# Archon

[![CI Pipeline](https://github.com/jameshughes89/archon/actions/workflows/ci-pipeline.yml/badge.svg)](https://github.com/jameshughes89/archon/actions/workflows/ci-pipeline.yml)
[![CD Pipeline](https://github.com/jameshughes89/archon/actions/workflows/cd-pipeline.yml/badge.svg)](https://github.com/jameshughes89/archon/actions/workflows/cd-pipeline.yml)
[![NuGet](https://img.shields.io/nuget/v/Archon.svg)](https://www.nuget.org/packages/Archon/)

Roslyn analysers for enforcing architectural rules in C# projects.

## Installation

```bash
dotnet add package Archon
```

## Features

- Enforce architectural boundaries
- Validate dependency rules
- Ensure consistent code organisation

## Usage

Once installed, the analysers will automatically run during compilation and highlight violations in your IDE.

## Configuration

Configure severity levels in your `.editorconfig`:

```editorconfig
[*.cs]
dotnet_diagnostic.ARCHON001.severity = error
```

## Development

### Prerequisites

- .NET 10.0 SDK or later
- (Optional) `act` for local CI/CD testing

### Building

```bash
cd src
dotnet restore Archon.slnx
dotnet build Archon.slnx
```

### Testing

```bash
cd src/Archon.Tests.Unit/bin/Release/net10.0
dotnet Archon.Tests.Unit.dll
```

### Local CI/CD Testing

Test the CI pipeline locally:
```bash
./scripts/test-ci.sh
```

Test the CD pipeline locally:
```bash
./scripts/test-cd.sh
```

## Licence

This project is licensed under the MIT Licence - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
