# SwaggerDescriptionBuilder

A fluent builder for creating structured Swagger/OpenAPI endpoint descriptions with markdown formatting.

## Overview

`SwaggerDescriptionBuilder` provides a thread-safe, fluent API for building well-formatted descriptions for Swagger/OpenAPI documentation. It generates markdown-formatted text with titles (as headers) and key-value tags (as list items).

## Features

- **Fluent API** - Method chaining for easy composition
- **Thread-Safe** - Safe for concurrent use across multiple threads
- **Markdown Output** - Generates clean, structured markdown
- **Validation** - Prevents invalid characters (hyphens and hash symbols)
- **Unicode Support** - Full support for international characters and emojis
- **.NET 9** - Built for modern .NET

## Installation

```bash
dotnet add package GUtils
```

## Quick Start

### Basic Usage

```csharp
using GUtils.SwaggerDescriptionBuilder;

var description = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("User Management API")
    .WithTag("Author", "Glen Wilkin")
    .WithTag("Version", "1.0.0")
    .Build();

// Output:
// ## User Management API
// - Author: Glen Wilkin
// - Version: 1.0.0
```

### Multiple Sections

```csharp
var description = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("Overview")
    .WithTag("Description", "Manage user accounts")
    .WithTag("Authentication", "Bearer token required")
    .WithTitle("Contact")
    .WithTag("Email", "support@example.com")
    .WithTag("GitHub", "https://github.com/user/repo")
    .Build();

// Output:
// ## Overview
// - Description: Manage user accounts
// - Authentication: Bearer token required
// ## Contact
// - Email: support@example.com
// - GitHub: https://github.com/user/repo
```

## API Reference

### Create()

Creates a new instance of `SwaggerDescriptionBuilder`.

```csharp
var builder = SwaggerDescriptionBuilder.Create();
```

**Returns:** A new `SwaggerDescriptionBuilder` instance.

### WithTitle(string description)

Adds a markdown header (##) to the description.

```csharp
builder.WithTitle("API Documentation");
```

**Parameters:**
- `description` - The title text (cannot contain hyphens `-`)

**Returns:** The same builder instance for chaining.

**Throws:**
- `ArgumentNullException` - If description is null
- `ArgumentException` - If description contains hyphens

### WithTag(string tagName, string tagValue)

Adds a key-value pair as a markdown list item.

```csharp
builder.WithTag("Author", "Glen Wilkin");
```

**Parameters:**
- `tagName` - The tag name (cannot contain hyphens `-` or hash symbols `#`)
- `tagValue` - The tag value (cannot contain hyphens `-` or hash symbols `#`)

**Returns:** The same builder instance for chaining.

**Throws:**
- `ArgumentNullException` - If tagName or tagValue is null
- `ArgumentException` - If tagName or tagValue contains hyphens or hash symbols

### Clear()

Resets the builder to an empty state, allowing it to be reused.

```csharp
builder.Clear();
```

**Returns:** The same builder instance for chaining.

**Notes:**
- Removes all previously added titles and tags
- Does not affect strings previously returned by `Build()`
- Useful for reusing the same builder instance multiple times
- Thread-safe like all other methods

### Build()

Generates the final markdown-formatted description string.

```csharp
string description = builder.Build();
```

**Returns:** The complete formatted description string.

**Notes:**
- Can be called multiple times on the same builder instance
- Returns the current state each time
- Builder remains mutable after calling Build()

## Usage Examples

### With FastEndpoints

```csharp
public class MyEndpoint : EndpointWithoutRequest<MyResponse>
{
    public override void Configure()
    {
        var builder = SwaggerDescriptionBuilder
            .Create()
            .WithTitle("My API Endpoint")
            .WithTag("Category", "Users")
            .WithTag("Version", "2.0")
            .WithTitle("Authentication")
            .WithTag("Required", "Yes")
            .WithTag("Type", "Bearer Token");

        this.Get("/api/myendpoint");
        this.Summary(s =>
        {
            s.Summary = "My endpoint summary";
            s.Description = builder.Build();
        });
    }
}
```

### Real-World Example

```csharp
var description = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("ClassCopier API Documentation")
    .WithTag("Purpose", "Deep copy objects in .NET")
    .WithTag("Author", "Glen Wilkin")
    .WithTitle("Getting Started")
    .WithTag("Installation", "dotnet add package GUtils")
    .WithTag("Usage", "ClassCopier.DeepCopy(myObject)")
    .WithTitle("Additional Resources")
    .WithTag("GitHub", "https://github.com/WilkinGlen/GUtils")
    .WithTag("License", "MIT")
    .Build();
```

**Output:**
```markdown
## ClassCopier API Documentation
- Purpose: Deep copy objects in .NET
- Author: Glen Wilkin
## Getting Started
- Installation: dotnet add package GUtils
- Usage: ClassCopier.DeepCopy(myObject)
## Additional Resources
- GitHub: https://github.com/WilkinGlen/GUtils
- License: MIT
```

### Unicode and Special Characters

```csharp
var description = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("API ?? ??")
    .WithTag("??", "Glen Wilkin ?????")
    .WithTag("Description", "Supports **bold** and *italic* markdown")
    .WithTag("Link", "[GitHub](https://github.com)")
    .Build();
```

### Incremental Building

```csharp
var builder = SwaggerDescriptionBuilder.Create();

// Build incrementally
builder.WithTitle("Initial Title");
var stage1 = builder.Build();

builder.WithTag("Tag1", "Value1");
var stage2 = builder.Build();

builder.WithTitle("Another Section");
var final = builder.Build();

// stage1, stage2, and final each contain progressively more content
```

### Reusing a Builder with Clear()

```csharp
var builder = SwaggerDescriptionBuilder.Create();

// First description
builder
    .WithTitle("API v1")
    .WithTag("Version", "1.0");
var v1Description = builder.Build();

// Clear and create second description
builder
    .Clear()
    .WithTitle("API v2")
    .WithTag("Version", "2.0");
var v2Description = builder.Build();

// v1Description and v2Description are completely independent
```

## Validation Rules

### Character Restrictions

The following characters are **not allowed**:

- **Hyphens (`-`)** in titles, tag names, or tag values
- **Hash symbols (`#`)** in tag names or tag values

These restrictions ensure the generated markdown is properly formatted and parseable.

### Examples of Invalid Input

```csharp
// ? Will throw ArgumentException
builder.WithTitle("My-Title");          // Hyphen in title
builder.WithTag("Tag-Name", "Value");   // Hyphen in tag name
builder.WithTag("Tag", "My-Value");     // Hyphen in tag value
builder.WithTag("Tag#Name", "Value");   // Hash in tag name
builder.WithTag("Tag", "Value#123");    // Hash in tag value

// ? Valid alternatives
builder.WithTitle("My Title");          // Space instead of hyphen
builder.WithTag("TagName", "Value");    // No hyphen
builder.WithTag("Tag", "My Value");     // Space instead of hyphen
builder.WithTag("Tag_Name", "Value");   // Underscore allowed
builder.WithTag("Tag", "Value 123");    // Space instead of hash
```

## Thread Safety

`SwaggerDescriptionBuilder` is **thread-safe** for:
- ? Concurrent operations on the **same instance** (protected by locks)
- ? Concurrent operations on **different instances** (no shared state)

### Thread-Safe Usage

```csharp
// Safe: Multiple threads using the same builder instance
var builder = SwaggerDescriptionBuilder.Create();

Parallel.For(0, 100, i =>
{
    builder.WithTitle($"Section {i}");
    builder.WithTag($"Tag{i}", $"Value{i}");
});

var result = builder.Build();
```

```csharp
// Safe: Multiple threads with separate builder instances
var builders = Enumerable.Range(0, 100)
    .Select(_ => SwaggerDescriptionBuilder.Create())
    .ToList();

Parallel.ForEach(builders, (builder, state, index) =>
{
    builder
        .WithTitle($"Title {index}")
        .WithTag($"Tag{index}", $"Value{index}");
});

var results = builders.Select(b => b.Build()).ToList();
```

## Output Format

The builder generates markdown in the following format:

```markdown
## [Title Text]
- [TagName1]: [TagValue1]
- [TagName2]: [TagValue2]
## [Another Title]
- [TagName3]: [TagValue3]
```

Each title becomes a level-2 markdown header (`##`), and each tag becomes a markdown list item (`-`).

## Best Practices

### ✅ Do

- Use the fluent API for clean, readable code
- Group related tags under appropriate titles
- Use descriptive tag names
- Call `Build()` only when you need the final string
- Reuse builder instances with `Clear()` when building similar descriptions
- Use `Clear()` in loops to avoid creating multiple builder instances

### ❌ Don't

- Use hyphens or hash symbols in titles or tags (will throw exception)
- Assume the builder is reset after calling `Build()` (it's not - use `Clear()` instead)
- Create new builder instances unnecessarily when `Clear()` would suffice

## Common Patterns

### API Documentation Header

```csharp
var header = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("Welcome to the API")
    .WithTag("Base URL", "https://api.example.com/v1")
    .WithTag("Authentication", "Bearer token required")
    .WithTag("Rate Limit", "1000 requests per hour")
    .WithTitle("Support")
    .WithTag("Email", "support@example.com")
    .WithTag("Documentation", "https://docs.example.com")
    .Build();
```

### Version History

```csharp
var versions = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("Version History")
    .WithTag("v2.0.0", "Breaking changes 2024/03/01")
    .WithTag("v1.1.0", "Added new features 2024/02/01")
    .WithTag("v1.0.0", "Initial release 2024/01/01")
    .Build();
```

### Endpoint Metadata

```csharp
var metadata = SwaggerDescriptionBuilder
    .Create()
    .WithTitle("Endpoint Details")
    .WithTag("Method", "POST")
    .WithTag("Content Type", "application/json")
    .WithTag("Response Code", "201 Created")
    .WithTag("Rate Limited", "Yes")
    .Build();
```

### Reusable Builder Pattern

```csharp
var builder = SwaggerDescriptionBuilder.Create();
var descriptions = new List<string>();

// Generate multiple descriptions efficiently
var endpoints = new[] { "Users", "Products", "Orders" };
foreach (var endpoint in endpoints)
{
    builder
        .WithTitle($"{endpoint} API")
        .WithTag("Category", endpoint)
        .WithTag("Author", "Glen Wilkin")
        .WithTag("Version", "1.0");
    
    descriptions.Add(builder.Build());
    builder.Clear(); // Reset for next iteration
}

// descriptions[0] = Users API description
// descriptions[1] = Products API description  
// descriptions[2] = Orders API description
```

## FAQ

### Q: Can I call `Build()` multiple times?
**A:** Yes! `Build()` returns the current state and can be called repeatedly. The builder remains mutable after each call.

### Q: Are builder instances reusable?
**A:** Yes! You can reuse a builder instance by calling `Clear()` to reset it to an empty state. Without calling `Clear()`, the builder is **cumulative** - each call to `WithTitle()` or `WithTag()` adds to the existing content.

```csharp
var builder = SwaggerDescriptionBuilder.Create();

// First use
builder.WithTitle("Title1").WithTag("Tag1", "Value1");
var first = builder.Build();

// Reuse with Clear()
builder.Clear().WithTitle("Title2").WithTag("Tag2", "Value2");
var second = builder.Build();

// first contains only Title1/Tag1, second contains only Title2/Tag2
```

### Q: Why can't I use hyphens?
**A:** Hyphens are reserved for the markdown list item syntax (`- TagName: TagValue`). Allowing them in content would break parsing.

### Q: Why can't I use hash symbols?
**A:** Hash symbols are reserved for markdown header syntax (`## Title`). Allowing them in content would interfere with title parsing.

### Q: Is it thread-safe?
**A:** Yes! Both single-instance concurrent use and multi-instance concurrent use are safe.

### Q: Can I include markdown in tag values?
**A:** Yes! You can include markdown formatting like `**bold**`, `*italic*`, or `[links](url)` in tag values.

## Related Components

- **SwaggerDescriptionInterrogator** - Parses descriptions back into structured data
- **ApiPathDescription** - Data model for parsed descriptions

## License

MIT License - see the [LICENSE](../../LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

- **GitHub Issues:** [https://github.com/WilkinGlen/GUtils/issues](https://github.com/WilkinGlen/GUtils/issues)
- **Author:** Glen Wilkin
