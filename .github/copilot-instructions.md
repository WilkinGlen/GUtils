# GUtils AI Coding Instructions

## Project Overview

GUtils is a multi-targeted NuGet library (.NET 8 & 9) providing high-performance utilities: ClassCopier (JSON-based deep cloning), SwaggerDescriptionBuilder (fluent Swagger description API), and SwaggerDescriptionInterrogator (markdown parser for Swagger descriptions).

## Repository Structure

- `GUtils/` - Main NuGet library (multi-targeted: net8.0;net9.0)
  - `ClassCopier/` - Deep object cloning using Newtonsoft.Json
  - `SwaggerDescriptionBuilder/` - Thread-safe fluent API for Swagger descriptions
  - `SwaggerDescriptionInterrogator/` - Span-based parser for Swagger JSON
- `GUtils.Api/` - Demo FastEndpoints API showcasing utilities (net9.0)
- `GUtilsTests/` - xUnit test suite (293+ tests, net9.0)
- `GUtilsTester/` - Manual testing console app (net8.0)
- `nupkgs/` - NuGet package output directory

## Multi-Targeting Implementation

**CRITICAL:** GUtils supports .NET 8 and .NET 9 using conditional compilation:

```csharp
#if NET9_0_OR_GREATER
    private readonly Lock myLock = new();  // .NET 9+ optimized lock
#else
    private readonly object myLock = new();  // .NET 8 compatible
#endif
```

- Always use conditional compilation when introducing .NET 9+ APIs
- Test changes against both target frameworks
- The demo API and tests target net9.0 only; the library targets both

## Key Design Patterns

### Thread Safety
ALL utilities are thread-safe. Lock-based synchronization protects shared state in `SwaggerDescriptionBuilder` and `ClassCopier.IgnoreDelegateContractResolver`.

### Performance-First Approach
- Span-based parsing in `SwaggerDescriptionInterrogator` (minimal allocations)
- Shared `JsonSerializerSettings` in `ClassCopier` (avoid repeated initialization)
- Caching in `IgnoreDelegateContractResolver` for reflection lookups

### Fluent API Design
`SwaggerDescriptionBuilder` follows method chaining pattern:
```csharp
SwaggerDescriptionBuilder
    .Create()
    .WithTitle("Section")
    .WithTag("Key", "Value")
    .Build();
```

### Validation Philosophy
- Strict validation: hyphens (`-`) and hash (`#`) are FORBIDDEN in titles/tags (prevents markdown parsing conflicts)
- Null checks with `ArgumentNullException.ThrowIfNull`
- Whitespace validation for tag names/values

## Development Workflows

### Building & Testing
```powershell
# Build library (both frameworks)
dotnet build GUtils\GUtils.csproj --configuration Release

# Run all tests
dotnet test

# Create NuGet package
dotnet pack GUtils\GUtils.csproj --configuration Release --output .\nupkgs
```

### Running Demo API
The `GUtils.Api` project demonstrates SwaggerDescriptionBuilder with FastEndpoints:
- Uses FastEndpoints + Scalar UI
- See `Endpoints/TestEndpoint.cs` for SwaggerDescriptionBuilder usage
- Configure via `Summary(s => s.Description = builder.Build())`

## Testing Standards

- Use xUnit + FluentAssertions
- Pattern: `MethodName_Should.cs` (e.g., `Build_Should.cs`)
- Test edge cases: thread safety, null handling, validation failures
- Performance tests verify allocations and throughput
- See `GUtilsTests/` for comprehensive examples (290+ tests)

## ClassCopier Deep Dive

### Architecture
JSON serialization-based deep cloning with custom `IgnoreDelegateContractResolver`:
1. Excludes delegates (non-serializable)
2. Handles private fields AND properties
3. Manages readonly properties via backing field detection
4. Caches reflection results for performance

### Backing Field Detection
```csharp
// Matches "_propertyName" or "_PropertyName" to "PropertyName"
private static bool IsBackingFieldForProperty(FieldInfo field, PropertyInfo property)
```

### Special Handling
- IPAddress: Custom converter (`IPAddressConverter`)
- Circular references: `ReferenceLoopHandling.Ignore`
- Type preservation: `TypeNameHandling.All`

## Swagger Components Integration

### Builder Output Format
```
## Title
- TagName: TagValue
```

### Interrogator Parsing
- Parses Swagger JSON `paths.*.*.description` fields
- Returns `List<ApiPathDescription>` with `Name`, `Titles[]`, `Tags{}`
- Handles all line endings (CRLF, LF, CR)
- Span-based for zero-copy parsing

### FastEndpoints Usage
```csharp
Summary(s => {
    s.Description = SwaggerDescriptionBuilder
        .Create()
        .WithTitle("Endpoint Purpose")
        .WithTag("Author", "Glen Wilkin")
        .Build();
});
```

## NuGet Package Configuration

Key `GUtils.csproj` settings:
- `<TargetFrameworks>net8.0;net9.0</TargetFrameworks>` - Multi-targeting
- `<GenerateDocumentationFile>true</GenerateDocumentationFile>` - XML docs
- `<IncludeSymbols>true</IncludeSymbols>` + `<SymbolPackageFormat>snupkg</SymbolPackageFormat>` - Debug symbols
- SourceLink enabled via `Microsoft.SourceLink.GitHub` package
- `<Deterministic>true</Deterministic>` - Reproducible builds

## Common Pitfalls

1. **Don't forget conditional compilation** when using .NET 9 APIs like `Lock`
2. **Never allow hyphens/hashes** in SwaggerDescriptionBuilder titles or tags
3. **Remember thread safety** - all utilities must remain thread-safe
4. **Test both frameworks** - changes must work on net8.0 AND net9.0
5. **Pack from solution root** - `dotnet pack` needs relative path to `GUtils\GUtils.csproj`

## File Organization Conventions

- One class per file matching the class name
- README.md in component folders (e.g., `SwaggerDescriptionBuilder/README.md`)
- Tests mirror source structure: `GUtilsTests/ClassCopierTests/`, `GUtilsTests/SwaggerDescriptionBuilderTests/`
- Test files named `<Method>_Should.cs` pattern
