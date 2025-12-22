# Archon

[![CI Pipeline](https://img.shields.io/github/actions/workflow/status/hughesjs/archon/ci-pipeline.yml?style=for-the-badge&logo=github)](https://github.com/hughesjs/archon/actions/workflows/ci-pipeline.yml)
[![CD Pipeline](https://img.shields.io/github/actions/workflow/status/hughesjs/archon/cd-pipeline.yml?style=for-the-badge&logo=github)](https://github.com/hughesjs/archon/actions/workflows/cd-pipeline.yml)
![NuGet Downloads](https://img.shields.io/nuget/dt/Archon?style=for-the-badge&logo=nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FArchon%2F)

Roslyn analysers for enforcing architectural rules in C# projects.

## Installation

```bash
dotnet add package Archon
```

## Features

Archon provides Roslyn analysers that enforce namespace-based architectural rules:

### ARCHON001: Internals Are Internal

Ensures that all types within namespaces containing `.Internal` are properly restricted with `internal` or `private` access modifiers. This prevents accidental exposure of internal implementation details.

- **Severity**: Error
- **Namespace Pattern**: `*.Internal*` (e.g., `MyApp.Internal`, `MyApp.Services.Internal`)
- **Allowed Modifiers**: `internal`, `private`, `private protected`
- **Special Handling**: Nested types are exempt if their containing type is already `internal` or `private`

### ARCHON002: Publics Are Public

Ensures that top-level types within namespaces containing `.Public` are appropriately exposed with `public` or `protected` access modifiers. This enforces discoverability of your public API surface.

- **Severity**: Warning
- **Namespace Pattern**: `*.Public*` (e.g., `MyApp.Public`, `MyApp.Api.Public`)
- **Required Modifiers**: `public`, `protected`, `protected internal`
- **Scope**: Only applies to top-level types (nested types are exempt)

## Usage

Once installed, the analysers will automatically run during compilation and highlight violations in your IDE.

## Configuration

Configure severity levels in your `.editorconfig`:

```editorconfig
[*.cs]
# Enforce internal types in .Internal namespaces (default: error)
dotnet_diagnostic.ARCHON001.severity = error

# Enforce public types in .Public namespaces (default: warning)
dotnet_diagnostic.ARCHON002.severity = warning
```

### Example

```csharp
namespace MyApp.Internal
{
    // ✅ Correct - internal type in .Internal namespace
    internal class InternalService { }

    // ❌ ARCHON001 violation - public type in .Internal namespace
    public class PublicService { }
}

namespace MyApp.Public
{
    // ✅ Correct - public type in .Public namespace
    public class PublicApi { }

    // ❌ ARCHON002 violation - internal type in .Public namespace
    internal class InternalApi { }
}
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
