namespace GUtils.SwaggerDescriptionBuilder;

using System.Buffers;

public sealed class SwaggerDescriptionBuilder
{
    private readonly Lock myLock = new();
    private static readonly SearchValues<char> ForbiddenTagCharacters = SearchValues.Create(['-', '#']);

    private string? description;

    private SwaggerDescriptionBuilder() => this.description = string.Empty;

    public static SwaggerDescriptionBuilder Create() => new();

    public SwaggerDescriptionBuilder WithTitle(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        // Titles only forbid hyphens (not hash symbols - those are allowed for markdown)
        if (description.Contains('-'))
        {
            throw new ArgumentException("Title cannot contain hyphens.", nameof(description));
        }

        lock (this.myLock)
        {
            this.description += $"## {description}{Environment.NewLine}";
        }

        return this;
    }

    public SwaggerDescriptionBuilder WithTag(string tagName, string tagValue)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        ArgumentNullException.ThrowIfNull(tagValue);

        if (string.IsNullOrWhiteSpace(tagName))
        {
            throw new ArgumentException("Tag name cannot be empty or whitespace.", nameof(tagName));
        }

        if (string.IsNullOrWhiteSpace(tagValue))
        {
            throw new ArgumentException("Tag value cannot be empty or whitespace.", nameof(tagValue));
        }

        ValidateTagCharacters(tagName, "Tag name", nameof(tagName));
        ValidateTagCharacters(tagValue, "Tag value", nameof(tagValue));

        lock (this.myLock)
        {
            this.description += $"- {tagName}: {tagValue}{Environment.NewLine}";
        }

        return this;
    }

    public SwaggerDescriptionBuilder Clear()
    {
        lock (this.myLock)
        {
            this.description = string.Empty;
        }

        return this;
    }

    public string Build()
    {
        lock (this.myLock)
        {
            return $"""{this.description}""";
        }
    }

#if NET8_0_OR_GREATER
    private static void ValidateTagCharacters(string value, string fieldName, string paramName)
    {
        var index = value.AsSpan().IndexOfAny(ForbiddenTagCharacters);
        if (index >= 0)
        {
            var forbiddenChar = value[index];
            var charDescription = forbiddenChar == '-' ? "hyphens" : "hash symbols";
            throw new ArgumentException($"{fieldName} cannot contain {charDescription}.", paramName);
        }
    }
#else
    private static void ValidateTagCharacters(string value, string fieldName, string paramName)
    {
        if (value.Contains('-'))
        {
            throw new ArgumentException($"{fieldName} cannot contain hyphens.", paramName);
        }

        if (value.Contains('#'))
        {
            throw new ArgumentException($"{fieldName} cannot contain hash symbols.", paramName);
        }
    }
#endif
}
