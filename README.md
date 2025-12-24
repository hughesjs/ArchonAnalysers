# ArchonAnalysers

[![CI Pipeline](https://img.shields.io/github/actions/workflow/status/hughesjs/ArchonAnalysers/ci-pipeline.yml?style=for-the-badge&logo=github)](https://github.com/hughesjs/ArchonAnalysers/actions/workflows/ci-pipeline.yml)
[![CD Pipeline](https://img.shields.io/github/actions/workflow/status/hughesjs/ArchonAnalysers/cd-pipeline.yml?style=for-the-badge&logo=github)](https://github.com/hughesjs/ArchonAnalysers/actions/workflows/cd-pipeline.yml)
[![NuGet](https://img.shields.io/nuget/v/ArchonAnalysers?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/ArchonAnalysers/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ArchonAnalysers?style=for-the-badge)](https://www.nuget.org/packages/ArchonAnalysers/)
[![License](https://img.shields.io/github/license/hughesjs/ArchonAnalysers?style=for-the-badge)](https://github.com/hughesjs/ArchonAnalysers/blob/master/LICENSE)
[![Made in Scotland](https://raw.githubusercontent.com/hughesjs/custom-badges/master/made-in/made-in-scotland.svg)](https://github.com/hughesjs/custom-badges)

Roslyn analysers for enforcing architectural rules in C# projects.

## Installation

```bash
dotnet add package ArchonAnalysers
```

## Features

ArchonAnalysers provides Roslyn analysers that enforce namespace-based architectural rules:

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

### Customising Namespace Patterns

Both analysers support customising which namespace patterns trigger the rules via `.editorconfig`:

#### ARCHON001: Internal Namespace Slugs

```editorconfig
[*.cs]
# Single namespace slug (default: Internal)
archon_001.internal_namespace_slugs = Internal

# Multiple namespace slugs
archon_001.internal_namespace_slugs = Internal, Private, Hidden, Impl
```

Types in namespaces matching `*.Internal.*`, `*.Private.*`, `*.Hidden.*`, or `*.Impl.*` must be internal, private, or private protected.

#### ARCHON002: Public Namespace Slugs

```editorconfig
[*.cs]
# Single namespace slug (default: Public)
archon_002.public_namespace_slugs = Public

# Multiple namespace slugs
archon_002.public_namespace_slugs = Public, Api, Exposed, Contract
```

Top-level types in namespaces matching `*.Public.*`, `*.Api.*`, `*.Exposed.*`, or `*.Contract.*` must be public, protected, or protected internal.

**Notes:**
- Slugs are comma-separated with automatic whitespace trimming
- Empty or missing configuration uses defaults ("Internal" for ARCHON001, "Public" for ARCHON002)
- Slugs match complete namespace segments (e.g., "Internal" matches `App.Internal.Services` but not `App.InternalStuff`)
- Special regex characters are automatically escaped

### Severity Configuration

Configure severity levels in your `.editorconfig`:

```editorconfig
[*.cs]
# Enforce internal types in configured namespaces (default: error)
dotnet_diagnostic.ARCHON001.severity = error

# Enforce public types in configured namespaces (default: warning)
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
cd src/ArchonAnalysers.Tests.Unit/bin/Release/net10.0
dotnet ArchonAnalysers.Tests.Unit.dll
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
